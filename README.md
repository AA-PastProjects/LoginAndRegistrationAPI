# LoginAndRegistrationAPI
Made in the summer of 2018 - It's a login and registration system that uses HTTPS and JWT tokens for security. - Made in .NET Core with C#, Swagger, Microsoft Identity, EntityFrameworkCore and your choice of MySQL or SQLServer (EFCore can use both).

### Setup of project - making it run

To make it work there are 2 steps you must take.

1. You need to set up the settings in the AppSettings.json file. More specifically these sections:

```
  "ConnectionStrings": {
    "DefaultConnection": "INSERT YOUR CONNECTION STRING HERE"
  },
  "Jwt": {
    "SecretKey": "INSERT JWT SECRET KEY HERE",
    "Audience": "INSERT JWT AUDIENCE SITE HERE",
    "Issuer": "INSERT JWT ISSUING SITE HERE"
  },
  "API": {
    "APIKey": "INSERT YOUR API KEY HERE OR GENERATE IT SOMEWHERE, CAN ALSO STORE IN DATABASE OR SIMILAR."
  }
```

2. In the Startup.cs file, change the options in ConfigureServices to use the SQL type of your choice, e.g. MySql or SqlServer.
Note that there are 2 mysql choices, MySQL or MySql, use MySql.

The application should now be able to run.

If you start it from Visual Studio you will end up at the website address it's hosted locally on (if you have IIS set up correctly).

There you can go to the web address /swagger to get information on how to consume the API, which request types it support, API endpoints, body's to send and so on.

### Further information

In another project I am currently working on, I have used a login system similar to this one to make role based authorization.