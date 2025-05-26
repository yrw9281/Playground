import { Routes } from '@angular/router';
import { loadRemoteModule } from '@angular-architects/native-federation';
import { HomeComponent } from './pages/home/home.component';

export const routes: Routes = [
    { path: '', component: HomeComponent },
    {
        path: 'mfe01',
        loadComponent: () =>
            loadRemoteModule('mfe01', './Component').then((m) => m.AppComponent),
    },
    {
        path: 'mfe02',
        loadComponent: () =>
            loadRemoteModule('mfe02', './Component').then((m) => m.AppComponent),
    },
    {
        path: '**',
        component: HomeComponent,
    },
];