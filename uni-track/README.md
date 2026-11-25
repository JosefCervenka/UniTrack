# UniTrack

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 17.3.4.

## Development server

Run `ng serve` for a dev server. Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Run ASP.NET and Angular together in Rider

Run `npm install` in this directory once to install the Angular dependencies. From the solution root, configure the backend connection string with `dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project Backend\Backend.csproj`.

In Rider, run the backend using the `http-with-angular` launch profile. It starts the ASP.NET backend first and launches `npm start` after the backend has started; Angular is available at `http://localhost:4200/`. Stopping the backend also stops the Angular development server. The existing `http` and `https` profiles do not start Angular.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
