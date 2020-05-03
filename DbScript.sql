-- Can be used to create the table manually

CREATE TABLE [dbo].[Images] (
    [ImageID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [ImageName]         NVARCHAR (MAX) NOT NULL,
    [ImageWebURL]       NVARCHAR (MAX) NULL,
    [ImageBlobId]       BIGINT         NULL,
    [Height]            INT            NOT NULL,
    [Width]             INT            NOT NULL,
    [ImageURLPrefix]    NVARCHAR (MAX) NULL,
    [ImageURLSuffix]    NVARCHAR (MAX) NULL,
    [FourSquareVenueId] NVARCHAR (MAX) NULL,
    [GooglePlaceId]     NVARCHAR (MAX) NULL,
    [FourSquareImageId] NVARCHAR (MAX) NULL,
    [GoogleImageId]     NVARCHAR (MAX) NULL,
    [ImageApiSource]    INT            NOT NULL,
    [Latitude]          FLOAT (53)     NULL,
    [Longitude]         FLOAT (53)     NULL,
    [City]              NVARCHAR (MAX) NULL,
    [Country]           NVARCHAR (MAX) NULL
);

CREATE TABLE [dbo].[ImageLocationMaps] (
    [ImageLocationMapId] BIGINT IDENTITY (1, 1) NOT NULL,
    [LocationId]         BIGINT NOT NULL,
    [ImageId]            BIGINT NOT NULL
);

CREATE TABLE [dbo].[ImageBlobs] (
    [ImageBlobId] BIGINT          IDENTITY (1, 1) NOT NULL,
    [Blob]        VARBINARY (MAX) NULL
);

CREATE TABLE [dbo].[Locations] (
    [LocationId]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [LocationName]         NVARCHAR (MAX) NOT NULL,
    [LongitudeDecimal]     FLOAT (53)     NULL,
    [LatitudeDecimal]      FLOAT (53)     NULL,
    [ImageLoadingComplete] BIT            NOT NULL,
    [LocationCountryCode]  NVARCHAR (MAX) NULL,
    [LastRunStatus]        NVARCHAR (MAX) NULL
);

CREATE TABLE [dbo].[UserLocationMaps] (
    [UserLocationMapId] BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserId]            BIGINT        NOT NULL,
    [LocationId]        BIGINT        NOT NULL,
    [AddedOn]           DATETIME2 (7) NOT NULL,
    [RemovedOn]         DATETIME2 (7) NULL
);

CREATE TABLE [dbo].[Users] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (MAX)  NULL,
    [LastName]     NVARCHAR (MAX)  NULL,
    [Username]     NVARCHAR (MAX)  NULL,
    [PasswordHash] VARBINARY (MAX) NULL,
    [PasswordSalt] VARBINARY (MAX) NULL
);
