# Image Aggregator

This image aggregator api reads from FourSquare and GooglePlaces for location images.
It is written entirely in C# in .net core 3.1

Its intended as an exploration of the .net core platform and implementation around the new technology stack. The heavy work around the images 
and locations is done in a background task manager, allowing the api to perform other actions without interruption. The current status is available
on the locations as well as the last status when an aggregation was run against it.

# Getting Started

Being .net core , the api should run on any environment able to host the .net core stack.

## Environment

Hosting SDKs and tools can be found [here]( https://dotnet.microsoft.com/download/dotnet-core/3.1): 

Deployment Instructions for various environments listed [here]( https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/?view=aspnetcore-3.1)

## Database
The appsettings.json file will need to be updated with your respective environmental settings. Currently the api is configured to 
utilise Microsoft Entity framework for persistance to a SQL database. The default configuration targets a SQL Server Express LocalDB which can be used
when testing but it is highly recommended that it is changed to a standard sql connection and security.

The system is also configured on a code first deployment so running it for the first time should initialise the database. However, the database and table creation scripts
have been included as well for those wishing to create it manually or on an alternate db layer.

## Security

There is a tag called `<AppSettings:Secrets>` which stores the salt used to generate the security token for authentication.

## Logging

Basic logging is enabled to the system logs

## 3rd Party API's

Currently the system caters for two thrid party API's. Both require an account to be created for use but then are simply configured via the respective settings
in the appsettings.json . At this time, **both API's keys are needed** for the API to function. Will consider better handling of seperate api services at a later date.

### FourSquare
```
  "FourSquare": {
    "ClientID": "Insert your client id here",
    "ClientSecret": "Insert your client secret here"
  }
```

An account can be created [here]( https://foursquare.com/developers/signup) and on creating your keys [here]( https://developer.foursquare.com/docs/places-api/getting-started/ ) 
  
### GooglePlaces
```
 "GoogleAPI": {
    "APIKey": "Insert your google api key here"
  }
```

Basic instructions on creating your key [here]( https://developers.google.com/places/web-service/get-api-key) and enableing the maping specifically [here](  https://developers.google.com/maps/gmp-get-started)

Note that billing must be enabled ( there is a very large free tier ) and the places api enabled.


# API Method Calls and Object Documentation

The API uses the popular swagger and swashbuckle to create a living document. This site is available at the route of the website and allows
users to experiment and play with the api as well as review the request and response models. 

The API has three main segments

## Locations

This segment deals with all location data, there are the basic methods to search for a location and retreive as well as the main method of retreiving images for a location. The **POST** method allows a location to be saved by either name or GPS
and a location can be submitted for a re-image seach. The system will not overwrite any existing images for a location and will attempt to cross correlate if existing stored images match.

The user location maps require authorisation to access. A user can create and account using the Register call. The user then authenticates themselves via the Authenticate call and receives a token. That token is required to be
passed in the header for any request requiring authorisation and the server will leverage the header when retreiving or commiting user sensitive data.

This allows the user to add items to a favourites list, retreive the list and remove items from the list. Note the removal does not delete the record but rather marks it.

## Images

As images tend to be heavy when shifting around, the data is split between two basic objects **Image** which contains the metadata and **ImageBlob** which is strictly just the byte data. 
There is an option to retreive a complex image object that combines both and any linked locations but only singlular. A search option has been added as well that searches against the three basic fields.

## User

This contains the Register and Authenticate calls.

# License

No license save for the relevant used components and third party apis. Please review both FourSquare and Googles acceptable use policy as well as be cognisant of the respective api limits.

# Contact

Feel free to leave a comment in the repository.
