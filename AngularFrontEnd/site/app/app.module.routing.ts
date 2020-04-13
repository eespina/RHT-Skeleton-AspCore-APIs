import { NgModule } from '@angular/core';
import { Router, RouterModule, Routes, Event, NavigationStart, NavigationEnd, NavigationError, NavigationCancel } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './user/register.component';
import { LoginComponent } from './user/login.component';
import { PageNotFoundComponent } from './shared/pageNotFound.component';
import { ExampleComponent } from './example/example.component';
import { ExampleListComponent } from './example/exampleList.component';
import { AuthGuard } from './user/auth-route-guard.service';
import { PreloaderService } from './shared/preloader.service';
import { AuthService } from './user/auth.service';

@NgModule({
    imports: [
        RouterModule.forRoot([
            { path: 'home', component: HomeComponent },
            {
                path: 'register', component: RegisterComponent, canActivate: [AuthGuard], canDeactivate: [AuthGuard]
            },
            { path: 'login', component: LoginComponent },
            {
                path: 'examples', component: ExampleListComponent, canActivate: [AuthGuard] //data: { preload: true } (used for PreloaderService),
            },
            { path: 'examples/:userName', component: ExampleComponent, canActivate: [AuthGuard] },//{ path: 'examples/:userName/:id', .....  - to use more parameters
            { path: '', redirectTo: '/home', pathMatch: 'full' },
            { path: '**', component: PageNotFoundComponent }    //Precedence matters, use this last as a'Catch All' route
            //, loadChildren: 'app/examples/example.module#ExampleModule' }, //AFTER refactoring to Feature Modules, use this to implement, if desired,
                    //Lazy Loading of Features in the future(ALSO, remove the 'path' attribute in the Feature Module's @ngModule RouterModule.forChild([ { path: [HERE] } ])..
                    //should actually leave ONLY the Children(but outside the 'children' attrubite(so, should have curlybrace - separated array of path routes afterward)))
        ])
            // add ALL options together (i.e. preloadingStrategy AND enableTracing)
        //, { enableTracing: true }) //enables NavigationStart, RoutesRecognized, NavigationEnd, NavigationCanceled, NavigationError which all can then be seen in the console
            //, { preloadingStrategy: PreloaderService })    //canLoad blocks pre loading
    ],
    providers: [AuthGuard, AuthService],
    exports: [RouterModule]  // this gives any importing module's declarations access to the Router directive when AppRoutingModule is imported
})
export class AppRoutingModule {
    constructor(private router: Router) {

    }
}