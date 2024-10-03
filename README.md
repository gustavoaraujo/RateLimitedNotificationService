# Rate Limit Notification Service

The Rate Limit Notification Service is a class library project in C# which can be plugged in different ways (primarily, it was made for a Console Application, but can be adapted for APIs, for example).

## Data storage

Since we can have a lot of notification types, this service is plugged to DynamoDB, so everytime a new notification type is created, it won't need to deploy or reload the application.

As for the notifications, they are stored in a cache. I decided to use ElasticCache, because of service's replicability. 
We're going to be able to have various instances of this service with one centralized (and elastic) cache.

## Configuration

All of the configurations are on Config/appconfig.json.

CacheEndpoint: ElasticCache endpoint.
CachePort: ElasticCache port.
DbServiceURL: URl for DynamoDB.

## Unit tests

Unit tests are in a separate project. You can explore the possibilities of NotificationService.cs and RateLimiter.cs there.
Also, there's a demo project, where it's seen how users can invoke this service.