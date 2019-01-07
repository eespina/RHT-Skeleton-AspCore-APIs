import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { ExampleListComponent } from './example/exampleList.component';
import { RegisterComponent } from './user/register.component';
import { LoginComponent } from './user/login.component';
import { HeaderComponent } from './shared/header.component';
import { FooterComponent } from './shared/footer.component';
import { ExampleComponent } from './example/example.component';
import { ExampleCountComponent } from './example/exampleCount.component';
import { HttpModule } from '@angular/http';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';  //HttpClientModule
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './shared/pageNotFound.component';
import { ExampleService } from './example/example.service';
import { ExampleSingletonService } from './example/exampleSingleton.service';
import { AppRoutingModule } from './app.module.routing';
import { AuthService } from './user/auth.service';
import { AuthGuard } from './user/auth-route-guard.service';
import { TokenInterceptorService } from './user/token-interceptor.service';

@NgModule({
    imports: [
        HttpClientModule,
        BrowserModule,
        FormsModule,
        HttpModule,
        AppRoutingModule
        //InMemoryWebApiModule.forRoot()
    ],
    declarations: [
        AppComponent,
        ExampleListComponent,
        ExampleCountComponent,
        HomeComponent,
        LoginComponent,
        HeaderComponent,
        FooterComponent,
        RegisterComponent,
        PageNotFoundComponent,
        ExampleComponent],
    bootstrap: [
        AppComponent
    ],
    providers: [    // placed here because it is used in multiple components
        AuthService,
        AuthGuard,
        ExampleService,
        ExampleSingletonService,
        {
            provide: HTTP_INTERCEPTORS, useClass: TokenInterceptorService, multi: true
        }
    ]
})
export class AppModule { }