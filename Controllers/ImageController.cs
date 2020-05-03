using ImmageAggregatorAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImmageAggregatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IAAPIDbContext _context;

        public ImageController(IAAPIDbContext context)
        {
            _context = context;
        }



        /// <summary>
        /// Retreives a full image ( meta, linked locations and blob )
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns>An complex image</returns>
        /// <response code="200">Returns the image</response>
        /// <response code="404">No image found</response>    
        [HttpGet]
        public async Task<ActionResult<ImageComplex>> GetImage(long imageId)
        {
            var x = await Task.Run(() =>
                   from i in _context.Images
                   join ib in _context.ImageBlobs on i.ImageBlobId equals ib.ImageBlobId into blob
                   from b in blob.DefaultIfEmpty()
                   where i.ImageId == imageId
                   select new ImageComplex
                   {
                       ImageMeta = i,
                       Blob = b,
                   });
            var result = x.FirstOrDefault();
            result.LinkedLocations = (await Task.Run(() =>
                                                         from ilm in _context.ImageLocationMaps
                                                         join l in _context.Locations on ilm.LocationId equals l.LocationId
                                                         where ilm.ImageId==imageId
                                                         select l)).ToList();


            return result;
        }

        /// <summary>
        /// Retreives just blob of an image
        /// </summary>
        /// <param name="imageBlobId"></param>
        /// <returns>A byte array of the data only</returns>
        /// <response code="200">Returns the byte array</response>
        /// <response code="404">No image found</response>  
        [HttpGet("BLOB/{imageBlobId}")]
        public async Task<ActionResult<byte[]>> GetImageBlobOnly(long imageBlobId)
        {
            var blob = await _context.ImageBlobs.FindAsync(imageBlobId);
            if (blob != null)
                return Ok(blob.Blob);
            else return NotFound();
        }

        /// <summary>
        /// Retreives an image's Meta data
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns>An image's meta data only</returns>
        /// <response code="200">Returns the meta data</response>
        /// <response code="404">No image found</response>  
        [HttpGet("MetaOnly/{imageId}")]
        public async Task<ActionResult<Image>> GetImageMetaOnly(long imageId)
        {
            return await _context.Images.FindAsync(imageId);
        }

        /// <summary>
        /// Searches amongst the saved images by the search terms (Max 20)
        /// </summary>
        /// <param name="imageName"></param>
        /// <param name="city"></param>
        /// <param name="countryCode"></param>
        /// <returns>A list of saved images (Max of 20 based off query)</returns>
        /// <response code="200">Returns the list</response>
        /// <response code="404">No Locations were found</response>    
        [HttpGet("Find")]
        public async Task<ActionResult<List<Image>>> Find(string imageName, string city,string countryCode)
        {
            var locations = _context.Images.Where(x=>1==1);
            if (!string.IsNullOrEmpty(imageName))
                locations = locations.Where(x => x.ImageName.StartsWith(imageName));
            if (!string.IsNullOrEmpty(city))
                locations = locations.Where(x => x.City.StartsWith(city));
            if (!string.IsNullOrEmpty(countryCode))
                locations = locations.Where(x => x.Country == countryCode);

            var searchResult = await Task.Run(()=>locations.Take(20).ToList());


            if (searchResult == null)
            {
                return NotFound();
            }

            return searchResult;
        }

        #region Not utilised Methods

        ///// <summary>
        ///// Saves an Image
        ///// </summary>
        ///// <returns>An saved image's id</returns>
        ///// <response code="200">Returns the saved image data</response>
        ///// <response code="500">Error while saving</response>  
        //[HttpPost]
        //public async Task<ActionResult<long>> Post(Image image)
        //{
        //    var newImage = await _context.Images.AddAsync(image);
        //    return CreatedAtRoute(new { imageId = newImage.Entity.ImageID }, newImage.Entity);
        //}

        ///// <summary>
        ///// Updates an Image
        ///// </summary>
        ///// <returns>A boolean whether was succesful or not</returns>
        ///// <response code="200">Returns the saved image data</response>
        ///// <response code="500">Error while updating</response>  
        //[HttpPut]
        //public async Task<ActionResult<bool>> Put(Image image)
        //{
        //    var existingImage = await _context.Images.FindAsync(image.ImageID);
        //    //Set properties
        //    existingImage.FourSquareImageId = image.FourSquareImageId;
        //    existingImage.GoogleImageId = image.GoogleImageId;
        //    existingImage.Height = image.Height;
        //    existingImage.ImageBlobId = image.ImageBlobId;
        //    existingImage.ImageName = image.ImageName;
        //    existingImage.ImageURLPrefix = image.ImageURLPrefix;
        //    existingImage.ImageURLSuffix = image.ImageURLSuffix;
        //    existingImage.ImageWebURL = image.ImageWebURL;
        //    existingImage.VenueId = image.VenueId;
        //    existingImage.Width = image.Width;

        //    return false;
        //}
        #endregion
    }
}