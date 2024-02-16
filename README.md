# Hacker News API Client

## Description

This project is a RESTful API that allows retrieving details of the best n stories from the Hacker News API, sorted by their score.

The API provides two endpoints:
1. GET /stories/{n} - Get details of the best n stories.
2. GET /health - Check the server's health status.

## Installation and Setup

### Requirements
- .NET Core 3.1 or higher

### Installation
1. Clone the repository to your local machine:
   
    git clone https://github.com/yourusername/hacker-news-api-client.git
    
2. Navigate to the project directory:
   
    cd hacker-news-api-client
    
### Configuration
1. Open the appsettings.json file.
2. Make sure that the Hacker News API endpoints are correctly specified.
3. Customize other settings if needed.

## Running

1. Run the application using .NET CLI:
   
    dotnet run
    
2. After starting, the application will be available at http://localhost:5000.

## Usage

### Getting details of the best stories
GET /stories/{n}

Request parameters:
- n (required) - the number of stories to retrieve.

Example request:
GET /stories/10

Example response:
[
    {
        "title": "Title 1",
        "url": "http://example.com/1",
        "score": 100
    },
    {
        "title": "Title 2",
        "url": "http://example.com/2",
        "score": 90
    },
    ...
]

### Error Handling

In case of errors, the API returns the appropriate HTTP status code along with an error message.

## Contributions

If you find a bug or want to contribute improvements, please open an Issue or Pull Request on the GitHub repository.
