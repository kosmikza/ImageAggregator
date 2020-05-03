using ImmageAggregatorAPI.BackgroundTasks;
using ImmageAggregatorAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ImmageAggregatorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILogger<LocationsController> _logger;
        private readonly IAAPIDbContext _context;
        private static ConnectionHelper _connection;
        private readonly IConfiguration _config;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly CancellationToken _cancellationToken;
        private readonly IServiceScopeFactory _serviceScopeFactory;




        public LocationsController(ILogger<LocationsController> logger, IAAPIDbContext context, IHttpClientFactory clientFactory, IConfiguration config, IBackgroundTaskQueue taskQueue,
            IHostApplicationLifetime applicationLifetime, IServiceScopeFactory serviceProvider)
        {
            _logger = logger;
            _context = context;
            _connection = new ConnectionHelper(clientFactory);
            _config = config;
            _taskQueue = taskQueue;
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _serviceScopeFactory = serviceProvider;
        }


        /// <summary>
        /// Searches amongst the saved locations by the search term
        /// </summary>
        /// <param name="locationName"></param>
        /// <returns>A list of saved locations (Max of 20 based off query)</returns>
        /// <response code="200">Returns the list</response>
        /// <response code="404">No Locations were found</response>    
        [HttpGet("{locationName}")]
        public async Task<ActionResult<List<Location>>> Find(string locationName)
        {
            var locations = await _context.Locations.Where(x => x.LocationName.StartsWith(locationName)).Take(20).ToListAsync();

            if (locations == null)
            {
                return NotFound();
            }

            return locations;
        }

        /// <summary>
        /// Retreives a saved location
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>A saved location</returns>
        /// <response code="200">Returns the location</response>
        /// <response code="404">No Location found</response>    
        [HttpGet("{locationId:long}")]
        public async Task<ActionResult<Location>> Get(long locationId)
        {
            var location = await _context.Locations.FindAsync(locationId);

            if (location == null)
            {
                return NotFound();
            }



            return location;
        }

        /// <summary>
        /// Queue a location for image refresh
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>bool whether queued or not</returns>
        /// <response code="200">Queued Succesfully</response>
        /// <response code="404">Not Queued</response>    
        [HttpGet("Refresh")]
        public async Task<ActionResult<Location>> RefreshImagesforLocation(long locationId)
        {
            try
            {
                var location = await _context.Locations.FindAsync(locationId);

                #region Fire off the background task to fetch the Images
                _taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    var guid = Guid.NewGuid().ToString();

                    _logger.LogInformation(
                         "Queued Background Task {Guid} is starting.", guid);

                    bool taskDone = false;

                    while (!token.IsCancellationRequested && !taskDone)
                    {
                        try
                        {
                            await GetandStoreImages(location);
                            taskDone = true;
                        }
                        catch (OperationCanceledException e)
                        {
                            _logger.LogInformation(
                               $"Queued Background Task {guid} has failed. Exception { e.Message } - { e.InnerException} ");
                        }


                        if (taskDone)
                        {
                            _logger.LogInformation(
                                "Queued Background Task {Guid} is complete.", guid);
                        }
                        else
                        {
                            _logger.LogInformation(
                                "Queued Background Task {Guid} was cancelled.", guid);
                        }
                    }
                });
                #endregion

                return Ok(true);
            }
            catch
            {
                return NotFound("Unable to Queue");
            }
        }

        /// <summary>
        /// Retreives all locations (50 Max)
        /// </summary>
        /// <returns>All locations</returns>
        /// <response code="200">Returns the locations</response>
        [HttpGet]
        public async Task<ActionResult<List<Location>>> GetAll()
        {
            var location = await _context.Locations.Take(50).ToListAsync();

            if (location == null)
            {
                return NotFound();
            }

            return location;
        }

        /// <summary>
        /// Retreives a list of images for a location
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns>A list of saved locations</returns>
        /// <response code="200">Returns the list</response>
        /// <response code="404">No images were found</response>    
        [HttpGet("{locationId}/Images")]
        public async Task<ActionResult<List<Image>>> GetImagesforLocation(long locationId)
        {
            var images = await (
                from ilm in _context.ImageLocationMaps
                join i in _context.Images on ilm.ImageId equals i.ImageId
                join l in _context.Locations on ilm.LocationId equals l.LocationId
                where l.LocationId == locationId
                select i
                ).ToListAsync();
            if (images.Count() == 0)
                return NotFound("No images found for location.");
            else return Ok(images);
        }


        /// <summary>
        /// Saves a Location
        /// </summary>
        /// <remarks>
        /// The name is passed but there is a possibilty of an incorrect location, please also use base country code ( ie: ZA for South Africa ) if it requires further filtering or the co-ordinates.
        /// </remarks>
        /// <returns>A saved location id</returns>
        /// <response code="200">Returns the saved location data</response>
        /// <response code="302">The location already exists</response>
        /// <response code="500">Error while saving</response>  
        [HttpPost]
        public async Task<ActionResult<long>> Post(LocationDTO locationDTO)
        {
            if (String.IsNullOrEmpty(locationDTO.LocationName) || (!locationDTO.LatitudeDecimal.HasValue || !locationDTO.LongitudeDecimal.HasValue))
            {
                var existingLocation = _context.Locations.Where(x => x.LocationName == locationDTO.LocationName ||
                        (x.LongitudeDecimal.HasValue && x.LongitudeDecimal == locationDTO.LongitudeDecimal && x.LatitudeDecimal.HasValue && x.LatitudeDecimal == locationDTO.LatitudeDecimal)).FirstOrDefault();
                if (existingLocation == null)
                {

                    var location = new Location
                    {
                        LocationName = locationDTO.LocationName,
                        LatitudeDecimal = locationDTO.LatitudeDecimal,
                        LongitudeDecimal = locationDTO.LongitudeDecimal,
                        LocationCountryCode = locationDTO.LocationCountryCode
                    };

                    //We need some name to store so lets do a quick single result query against the foursquare places and store the first result from the co-ords as the name, the background task will fire as normal.
                    string apiurl = $"https://api.foursquare.com/v2/venues/search?client_id={_config.GetValue<string>("FourSquare:ClientID")}&client_secret={_config.GetValue<string>("FourSquare:ClientSecret")}&v={DateTime.Now:yyyyMMdd}" +
                                    $"&{(location.LatitudeDecimal.HasValue ? $"ll={location.LatitudeDecimal},{location.LongitudeDecimal}" : $"near={WebUtility.UrlEncode(String.IsNullOrEmpty(location.LocationCountryCode) ? location.LocationName : $"{location.LocationName}, {location.LocationCountryCode}")}")}" +
                                    $"&intent=browse&radius=10000&limit=1";
                    try
                    {
                        var venues = await _connection.GetAsync<Models.FourSquare.GetVenueResponse>(apiurl);
                        foreach (Models.FourSquare.Venue fsVenue in venues.response.venues)
                        { 
                            if (location.LatitudeDecimal.HasValue)
                            { 
                                location.LocationName = fsVenue.name;
                                location.LocationCountryCode=String.IsNullOrEmpty(location.LocationCountryCode)?fsVenue.location.cc:location.LocationCountryCode;
                            }
                            else
                            {
                                location.LatitudeDecimal = fsVenue.location.lat;
                                location.LongitudeDecimal = fsVenue.location.lng;
                            }
                        }
                        if (String.IsNullOrEmpty(location.LocationName))
                            return NotFound("Please re-submit,using a name or check the co-ordinates, no locations found for current values");
                        _context.Locations.Add(location);
                        await _context.SaveChangesAsync();

                        #region Fire off the background task to fetch the Images
                        _taskQueue.QueueBackgroundWorkItem(async token =>
                        {
                            var guid = Guid.NewGuid().ToString();

                            _logger.LogInformation(
                                 "Queued Background Task {Guid} is starting.", guid);

                            bool taskDone = false;

                            while (!token.IsCancellationRequested && !taskDone)
                            {
                                try
                                {
                                    await GetandStoreImages(location);
                                    taskDone = true;
                                }
                                catch (OperationCanceledException e)
                                {
                                    _logger.LogInformation(
                                       $"Queued Background Task {guid} has failed. Exception { e.Message } - { e.InnerException} ");

                                }


                                if (taskDone)
                                {
                                    _logger.LogInformation(
                                        "Queued Background Task {Guid} is complete.", guid);
                                }
                                else
                                {
                                    _logger.LogInformation(
                                        "Queued Background Task {Guid} was cancelled.", guid);
                                }
                            }
                        });
                        #endregion

                        return CreatedAtAction(nameof(Get), new { locationId = location.LocationId }, location);
                    }
                    catch(Exception x)
                    {
                        if (x.Message.Contains("failed_geocode"))
                            return NotFound("Please re-submit,using a name or check the co-ordinates, no locations found for current values");
                        else throw x;
                    }
                }
                else return StatusCode(302, "Location already exists");
            }
            else return BadRequest("Either the Name or Co-ordinates must be passed");
        }



        #region Authorised Calls for Users to manage their lists

        /// <summary>
        /// Gets a users favourite list
        /// </summary>
        /// <remarks>
        /// This call requires authorisation of the user to ensure persistance
        /// </remarks>
        /// <returns>Saved favourites</returns>
        /// <response code="200">Returns saved maps that are active</response>
        /// <response code="500">Error while creating</response>  
        [Authorize]
        [HttpGet("UserLocations")]
        public async Task<ActionResult<List<UserLocationMap>>> GetUsersLocations()
        {
            var favourites = await _context.UserLocationMaps.Where(x => x.UserId == Convert.ToInt64(User.Identity.Name) && !x.RemovedOn.HasValue).ToListAsync();
            if (favourites.Count() > 0)
                return favourites;
            else return NotFound("No list found for user");
        }

        /// <summary>
        /// Adds a location to a users favourite list
        /// </summary>
        /// <remarks>
        /// This call requires authorisation of the user to ensure persistance
        /// </remarks>
        /// <returns>Saved favourite</returns>
        /// <response code="200">Returns saved map</response>
        /// <response code="500">Error while creating</response>  
        [Authorize]
        [HttpPost("UserLocations")]
        public async Task<ActionResult<UserLocationMap>> PostUserLocationMap(UserLocationMapDTO map)
        {
            var storedMap = await _context.UserLocationMaps.Where(x => x.LocationId == map.LocationId && x.UserId == Convert.ToInt64(User.Identity.Name) && !x.RemovedOn.HasValue).FirstOrDefaultAsync();
            if (storedMap == null)
            {
                storedMap = new UserLocationMap
                {
                    AddedOn = DateTime.Now,
                    UserId = Convert.ToInt64(User.Identity.Name),
                    LocationId = map.LocationId,
                };
                _context.UserLocationMaps.Add(storedMap);
                await _context.SaveChangesAsync();
                return storedMap;
            }
            else return StatusCode(302, "Map already exists");
        }

        /// <summary>
        /// Removes a location from a users favourite list
        /// </summary>
        /// <remarks>
        /// This call requires authorisation of the user to ensure persistance
        /// </remarks>
        /// <returns>A boolean whether was succesful or not</returns>
        /// <response code="200">Returns true or false for success</response>
        /// <response code="500">Error while updating</response>  
        [Authorize]
        [HttpDelete("UserLocations")]
        public async Task<ActionResult> DeleteUserLocationMap(long userLocationMapId)
        {
            var storedMap = await _context.UserLocationMaps.Where(x => x.UserLocationMapId == userLocationMapId && x.UserId == Convert.ToInt64(User.Identity.Name) && !x.RemovedOn.HasValue).FirstOrDefaultAsync();
            if (storedMap != null)
            {
                storedMap.RemovedOn = DateTime.Now;
                _context.UserLocationMaps.Add(storedMap);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
            else return Ok(false);
        }

        #endregion



        //[Authorize]
        //[HttpGet("Test")]
        //public async Task<ActionResult<bool>> Test()
        //{


        //    return true;
        //}

        #region Private Methods
        /// <summary>
        /// Task that retreives and store image data for a location
        /// </summary>
        /// <remarks>
        /// This task has been seperated to allow for running in background thread by adding it to the task scheduling engine. Updates are stored against the location object
        /// </remarks>
        private async Task GetandStoreImages(Location passedLocation)
        {
            //Get some venues from FourSquare
            //Configure the FourSquare api string
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IAAPIDbContext>();
            var location = await context.Locations.FindAsync(passedLocation.LocationId);
            location.LastRunStatus = "Processing...";
            await context.SaveChangesAsync();
            try
            {
                string apiurl = $"https://api.foursquare.com/v2/venues/search?client_id={_config.GetValue<string>("FourSquare:ClientID")}&client_secret={_config.GetValue<string>("FourSquare:ClientSecret")}&v={DateTime.Now:yyyyMMdd}" +
                    $"&{(location.LatitudeDecimal.HasValue ? $"ll={location.LatitudeDecimal},{location.LongitudeDecimal}" : $"near={WebUtility.UrlEncode(String.IsNullOrEmpty(location.LocationCountryCode) ? location.LocationName : $"{location.LocationName}, {location.LocationCountryCode}")}")}" +
                    $"&intent=browse&radius=10000&limit=10";
                var venues = await _connection.GetAsync<Models.FourSquare.GetVenueResponse>(apiurl);
                //Respect the limits
                bool fourSquareAPILimitExceeded = false;
                //bool googleAPILimitExceeded = false; Havent seen this yet.
                foreach (Models.FourSquare.Venue fsVenue in venues.response.venues)
                {
                    #region Images
                    //Attempt Retreival from foursquare )
                    var existingImage = context.Images.Where(x => x.FourSquareVenueId == fsVenue.id && x.ImageApiSource == (int)ImageApiSource.FourSquare).FirstOrDefault();
                    if (existingImage == null && !fourSquareAPILimitExceeded)
                    {
                        apiurl = $"https://api.foursquare.com/v2/venues/{fsVenue.id}/photos?client_id={_config.GetValue<string>("FourSquare:ClientID")}&client_secret={_config.GetValue<string>("FourSquare:ClientSecret")}&v={DateTime.Now:yyyyMMdd}" +
                                 $"&group=venue&limit=10";
                        var photos = new Models.FourSquare.GetVenuePhotoResponse();
                        try
                        {
                            photos = await _connection.GetAsync<Models.FourSquare.GetVenuePhotoResponse>(apiurl);
                            foreach (Models.FourSquare.PhotoItem photoItem in photos.response.photos.items)
                            {
                                var image = new Image
                                {
                                    FourSquareVenueId = fsVenue.id,
                                    FourSquareImageId = photoItem.id,
                                    Height = photoItem.height,
                                    Width = photoItem.width,
                                    ImageName = fsVenue.name,
                                    ImageURLPrefix = photoItem.prefix,
                                    ImageURLSuffix = photoItem.suffix,
                                    ImageWebURL = $"{photoItem.prefix}original{photoItem.suffix}",
                                    ImageApiSource = (int)ImageApiSource.FourSquare,
                                    Longitude = fsVenue.location.lng,
                                    Latitude = fsVenue.location.lat,
                                    City = fsVenue.location.city,
                                    Country = fsVenue.location.cc
                                };
                                context.Images.Add(image);
                                await context.SaveChangesAsync();
                                context.ImageLocationMaps.Add(new ImageLocationMap
                                {
                                    ImageId = image.ImageId,
                                    LocationId = location.LocationId
                                });
                                await context.SaveChangesAsync();
                                //Try Retreive the Blob and store
                                try
                                {
                                    var blob = await _connection.GetAsyncByte(image.ImageWebURL);
                                    if (blob.Length > 0)
                                    {
                                        var imageBlob = new ImageBlob
                                        {
                                            Blob = blob
                                        };
                                        context.Add(imageBlob);
                                        await context.SaveChangesAsync();
                                        image.ImageBlobId = imageBlob.ImageBlobId;
                                        await context.SaveChangesAsync();
                                    }
                                }
                                catch (OperationCanceledException ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                        catch
                        {
                            fourSquareAPILimitExceeded = true;
                        }


                    }
                    else
                    {
                        if (existingImage!=null)
                        { 
                        var locMap = await context.ImageLocationMaps.Where(x => x.ImageId == existingImage.ImageId && x.LocationId == location.LocationId).FirstOrDefaultAsync();
                        if (locMap == null) // Add a mapping if doesn't exist but we only have one image record
                        {
                            context.ImageLocationMaps.Add(new ImageLocationMap
                            {
                                ImageId = existingImage.ImageId,
                                LocationId = location.LocationId
                            });
                            await context.SaveChangesAsync();
                        }
                        }
                    }
                    //Now lets do the same using the GoogleMaps tool ( we'll submit the FourSquare Info to Google for more images )
                    existingImage = context.Images.Where(x => x.ImageApiSource == (int)ImageApiSource.Google &&
                                       (x.FourSquareVenueId == fsVenue.id || (x.Latitude == fsVenue.location.lat && x.Longitude == fsVenue.location.lng))).FirstOrDefault();
                    if (existingImage == null)
                    {
                        Image image = null;
                        //For now lets just get the google image reference
                        apiurl = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={fsVenue.name}&inputtype=textquery&locationbias=circle:2000@{fsVenue.location.lat},{fsVenue.location.lng}&fields=photos,formatted_address,name,geometry&key={_config.GetValue<string>("GoogleAPI:APIKey")}";
                        var photos = await _connection.GetAsync<Models.GoogleMaps.GoogleMaps.GooglePlaceNameSearch>(apiurl);
                        if (photos.status.Contains("ZERO_RESULTS") || (photos.candidates.Count > 0 && photos.candidates[0].photos == null)) //If we can't retreive by name, lets try by co-ords
                        {
                            apiurl = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={ ( !String.IsNullOrEmpty(fsVenue.location.city)?fsVenue.location.city:"city")}&inputtype=textquery&locationbias=circle:2000@{fsVenue.location.lat},{fsVenue.location.lng}&fields=photos,formatted_address,name,geometry&key={_config.GetValue<string>("GoogleAPI:APIKey")}";
                            photos = await _connection.GetAsync<Models.GoogleMaps.GoogleMaps.GooglePlaceNameSearch>(apiurl);
                        }
                        if (photos.candidates != null && photos.candidates.Count() > 0 && photos.candidates[0].photos != null && photos.candidates[0].photos.Count() > 0)
                        {
                            var gphoto = photos.candidates[0].photos[0];
                            existingImage = context.Images.Where(x => x.ImageApiSource == (int)ImageApiSource.Google &&
                                       (x.FourSquareVenueId == fsVenue.id || (x.Latitude == photos.candidates[0].geometry.location.lat && x.Longitude == photos.candidates[0].geometry.location.lng))).FirstOrDefault();
                            if (existingImage == null)
                            {
                                image = new Image
                                {
                                    FourSquareVenueId = fsVenue.id,
                                    GoogleImageId = gphoto.photo_reference,
                                    Height = gphoto.height,
                                    Width = gphoto.width,
                                    ImageName = photos.candidates[0].name,
                                    ImageApiSource = (int)ImageApiSource.Google,
                                    Latitude = photos.candidates[0].geometry.location.lat,
                                    Longitude = photos.candidates[0].geometry.location.lng,
                                    City = fsVenue.location.city,
                                    Country = fsVenue.location.cc
                                };
                                context.Images.Add(image);
                                await context.SaveChangesAsync();
                                context.ImageLocationMaps.Add(new ImageLocationMap
                                {
                                    ImageId = image.ImageId,
                                    LocationId = location.LocationId
                                });
                                await context.SaveChangesAsync();
                                //Try Retreive the Blob and store
                                try
                                {
                                    //Use Googles Photo Url
                                    string googlePhotoUrl = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth={ (image.Width > 1600 ? 1600 : image.Width) }&photoreference={image.GoogleImageId}&key={_config.GetValue<string>("GoogleAPI:APIKey")}";
                                    var blob = await _connection.GetAsyncByte(googlePhotoUrl);
                                    if (blob.Length > 0)
                                    {
                                        var imageBlob = new ImageBlob
                                        {
                                            Blob = blob
                                        };
                                        context.Add(imageBlob);
                                        await context.SaveChangesAsync();
                                        image.ImageBlobId = imageBlob.ImageBlobId;
                                        await context.SaveChangesAsync();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                var locMap = await context.ImageLocationMaps.Where(x => x.ImageId == existingImage.ImageId && x.LocationId == location.LocationId).FirstOrDefaultAsync();
                                if (locMap == null) // Add a mapping if doesn't exist
                                {
                                    context.ImageLocationMaps.Add(new ImageLocationMap
                                    {
                                        ImageId = existingImage.ImageId,
                                        LocationId = location.LocationId
                                    });
                                    await context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                    else
                    {
                        var locMap = await context.ImageLocationMaps.Where(x => x.ImageId == existingImage.ImageId && x.LocationId == location.LocationId).FirstOrDefaultAsync();
                        if (locMap==null) // Add a mapping if doesn't exist but we only have one image record
                        {
                            context.ImageLocationMaps.Add(new ImageLocationMap
                            {
                                ImageId=existingImage.ImageId,
                                LocationId=location.LocationId
                            });
                            await context.SaveChangesAsync();
                        }
                    }
                    #endregion
                }
                //Update the location with the geodata if available

                location.LatitudeDecimal = !location.LatitudeDecimal.HasValue ? venues.response.geocode.feature.geometry.center.lat : location.LatitudeDecimal;
                location.LongitudeDecimal = !location.LongitudeDecimal.HasValue ? venues.response.geocode.feature.geometry.center.lng : location.LongitudeDecimal;
                location.ImageLoadingComplete = true;
                location.LocationCountryCode = String.IsNullOrEmpty(location.LocationCountryCode) ? venues.response.venues[0].location.cc : location.LocationCountryCode;
                location.LastRunStatus = "No Errors";
                await context.SaveChangesAsync();
            }
            catch(Exception x)
            {
                location.ImageLoadingComplete = true;
                location.LastRunStatus = $"Error {x.Message} - {x.InnerException}";
                await context.SaveChangesAsync();
                throw x;
            }
        }

        private bool LocationExists(long locationId) =>
        _context.Locations.Any(e => e.LocationId == locationId);
        #endregion

        #region Not Utilised Methods

        ///// <summary>
        ///// Updates a location
        ///// </summary>
        ///// <returns>A boolean whether was succesful or not</returns>
        ///// <response code="200">Returns true or false for success</response>
        ///// <response code="500">Error while updating</response>  
        //[HttpPut]
        //public async Task<IActionResult> Put(Location location)
        //{
        //    var storedLocation = await _context.Locations.FindAsync(location.LocationId);
        //    if (storedLocation == null)
        //    {
        //        return NotFound();
        //    }

        //    storedLocation.LocationName =location.LocationName;
        //    storedLocation.LatitudeDecimal = location.LatitudeDecimal;
        //    storedLocation.LongitudeDecimal = location.LongitudeDecimal;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException) when (!LocationExists(location.LocationId))
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}

        #endregion
    }
}