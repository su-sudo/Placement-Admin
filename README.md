
# Student Test Performance Management System

## Overview

The Student Test Performance Management System is a web application designed to manage and display students' test results and performance indicators. The system allows students to view their test scores and other performance metrics through a user-friendly interface.

## Features

- **User Authentication**: Secure login and registration for students.
- **Dashboard**: A home page displaying a summary of the student's performance.
- **Charts**: Visual representation of test scores using Chart.js.
- **Test Submission**: Allows students to take and submit tests.
- **Result Analysis**: Display of test results and performance metrics.
- **Data Management**: Admin functionality to add questions, options, and manage tests.

## Technologies Used

- **Backend**: ASP.NET Core
- **Frontend**: HTML, CSS, JavaScript, Bootstrap
- **Database**: SQL Server
- **Charting Library**: Chart.js

## Setup and Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/su-sudo/Placement-Admin.git
   cd Placement-Admin
Set Up the Database

Ensure SQL Server is installed and running.

Create a new database.

Run the provided SQL scripts to create tables and insert sample data.

Configure Connection String

Update the connection string in appsettings.json to match your database configuration.

Install .NET Core Hosting Bundle

Download and install the .NET Core Hosting Bundle on your IIS server.

Build and Run the Application

Open the project in Visual Studio.

Build the solution.

Run the application using IIS Express or configure it for IIS deployment.

Deployment to IIS
Publish the Project

Right-click the project in Visual Studio and select Publish.

Choose Folder as the publish target and specify the folder location.

Copy Published Files to IIS

Copy the published files to the desired location on your IIS server (e.g., C:\inetpub\wwwroot\PlacementAdmin).

Create an IIS Site

Open IIS Manager on your server.

Right-click the Sites folder and select Add Website.

Provide a Site name, set the Physical path to the folder containing your published files, and configure the Binding settings (e.g., hostname, port).

Click OK to create the site.

Configure Application Pool

In IIS Manager, click on Application Pools.

Select the application pool associated with your site.

Set the .NET CLR version to No Managed Code (for .NET Core applications).

Set the Managed pipeline mode to Integrated.

Usage
Login as a Student

Navigate to the login page and enter your credentials.

View Performance Dashboard

Access the home page to view a summary of your performance indicators.

Take and Submit Tests

Navigate to the test page, complete the test, and submit your answers.

View Test Results

Access the results page to see your test scores and detailed performance analysis.

Contributing
Fork the repository.

Create a new branch (git checkout -b feature-branch).

Make your changes.

Commit your changes (git commit -am 'Add new feature').

Push to the branch (git push origin feature-branch).

Create a new Pull Request.

License
This project is licensed under the MIT License.

Acknowledgments
Microsoft: For providing the development tools and frameworks used in this project.

Chart.js: For the charting library used to display performance metrics.
