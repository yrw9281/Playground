# Angular Micor-Front-End POC - native-federation

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 18.2.9.

```bash
# Create workspace
ng new angular-mfe --create-application=false
# cd workspace
cd .\angular-mfe\
# Install native-federation
npm i -D @angular-architects/native-federation
```

## Create shell application

```bash
# Create shell application
ng generate application shell --prefix app-shell
# Initialize host(shell) application
ng g @angular-architects/native-federation:init --project shell --port 4200 --type dynamic-host
```

## Create new remote(MFE) applications

```bash
# Create new remote(MFE) application 01
ng generate application mfe01 --prefix app-mfe01
# Initialize remote(MFE) application 01
ng g @angular-architects/native-federation:init --project mfe01 --port 4201 --type remote

# Create new remote(MFE) application 02
ng generate application mfe02 --prefix app-mfe02
# Initialize remote(MFE) application 02
ng g @angular-architects/native-federation:init --project mfe02 --port 4202 --type remote
```

## Adding Routes in Shell

### Generate component

```bash
ng g c pages/home --project shell
```

### App route

Add below code into `shell/src/app/app.routes.ts`

```typescript
import { Routes } from "@angular/router";
import { loadRemoteModule } from "@angular-architects/native-federation";
import { HomeComponent } from "./pages/home/home.component";

export const routes: Routes = [
  { path: "", component: HomeComponent },
  {
    path: "mfe01",
    loadComponent: () =>
      loadRemoteModule("mfe01", "./Component").then((m) => m.AppComponent),
  },
  {
    path: "mfe02",
    loadComponent: () =>
      loadRemoteModule("mfe02", "./Component").then((m) => m.AppComponent),
  },
  {
    path: "**",
    component: HomeComponent,
  },
];
```

### App component

Add below code into `shell/src/app/app.component.html`

```html
<ul>
  <li><a routerLink="/">Shell</a></li>
  <li><a routerLink="/mfe01">MFE-01</a></li>
  <li><a routerLink="/mfe02">MFE-02</a></li>
</ul>
<router-outlet></router-outlet>
```

Import the router dependencies `shell/src/app/app.routes.ts`

```typescript
import { CommonModule } from "@angular/common";
import { Component } from "@angular/core";
import { RouterOutlet, RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: "app-shell-root",
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  title = "shell";
}
```

## Setup federation manifest

Modify the manifest in `shell/public/federation.manifest.json`

```json
{
  "mfe01": "http://localhost:4201/remoteEntry.json",
  "mfe02": "http://localhost:4202/remoteEntry.json"
}
```

Make sure the shell can access the manifest. `shell/src/main.ts`

```typescript
import { initFederation } from '@angular-architects/native-federation';

initFederation('/federation.manifest.json')
  .catch(err => console.error(err))
  .then(_ => import('./bootstrap'))
  .catch(err => console.error(err));
```

## Development server

Run `ng s shell` for a host and shell server.

Run `ng s mfe01` for running mfe01 in `http://localhost:4201/`.

Run `ng s mfe02` for running mfe02 in `http://localhost:4202/`.

Navigate to `http://localhost:4200/`. The application will automatically reload if you change any of the source files.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `ng build` to build the project. The build artifacts will be stored in the `dist/` directory.

## Running unit tests

Run `ng test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via a platform of your choice. To use this command, you need to first add a package that implements end-to-end testing capabilities.

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.
