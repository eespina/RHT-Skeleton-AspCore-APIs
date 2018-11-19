import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AppComponent } from './app.component';
import { ExampleListComponent } from './example/exampleList.component';
import { ExampleComponent } from './example/example.component';
import { ExampleCountComponent } from './example/exampleCount.component';
import { HttpModule } from '@angular/http';
import { HomeComponent } from './home/home.component';
import { PageNotFoundComponent } from './shared/pageNotFound.component';
import { ExampleService } from './example/example.service';
import { ExampleSingletonService } from './example/exampleSingleton.service';
import { AppRoutingModule } from './app.module.routing';

@NgModule({
    imports: [
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
        PageNotFoundComponent,
        ExampleComponent],
    bootstrap: [
        AppComponent
    ],
    providers: [    // placed here because it is used in multiple components
        ExampleService,
        ExampleSingletonService
    ]
})
export class AppModule { }