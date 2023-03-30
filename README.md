# Google Calendar Integration
This project is an integration with the Google Calendar API, which allows you to retrieve and manipulate events in your Google Calendar.

## Getting Started
To use this project, you will need to obtain a client ID and secret key from the Google Cloud Console. You can follow these steps to obtain your credentials:

1. Go to the  [Google Cloud Console](https://console.cloud.google.com/).
2. Create a new project or select an existing project.
3. Navigate to the API & Services section and click on the "Credentials" tab.
4. Click on the "Create credentials" button and select "OAuth client ID".
5. Follow the instructions to set up your OAuth consent screen and create your client ID.
6. Once your client ID is created, click on the "Download" button to download your credentials as a JSON file.
7. Move the downloaded file to the location specified in the credentials.json file.

You will also need to obtain an API key for the Google Calendar API. You can follow these steps to obtain your API key:
1. Go to the  [Google Cloud Console](https://console.cloud.google.com/).
2. Create a new project or select an existing project.
3. Navigate to the API & Services section and click on the "Credentials" tab.
4. Click on the "Create credentials" button and select "API key".
5. Follow the instructions to create your API key.
6. Once your API key is created, copy the key and paste it into the GoogleApiKey setting in the appsettings.json file.

## Running the Application
To run the application, you can use the following steps:
1. Clone the repository to your local machine.
2. Open the project in Visual Studio.
3. Update the values in the appsettings.json file and credentials.json file with your own credentials and settings.
4. Run the application in Visual Studio.

## Usage
The project will prompt you to authenticate with your Google account and allow the application to access your calendar. Once authenticated, Use the web interface to create, view, update, and delete calendar events.







