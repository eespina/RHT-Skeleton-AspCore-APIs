import { Component, OnInit } from '@angular/core';
import { IExample } from './example';
import { ActivatedRoute, Router } from '@angular/router';
import { ExampleService } from './example.service';
import 'rxjs/add/operator/retrywhen';
import 'rxjs/add/operator/delay';    //specifies the delay in mlliseconds when using retry
import 'rxjs/add/operator/scan';    //used for customizing the retry process
import { ISubscription } from 'rxjs/Subscription'

@Component({
    selector: 'example-detail',
    templateUrl: 'app/example/example.component.html',
    styleUrls: ['app/example/example.component.css']
})
export class ExampleComponent implements OnInit {
    example: IExample;
    statusMessage: string = 'Loading Data, Please Wait ...';
    subscription: ISubscription;

    constructor(private _exampleService: ExampleService, private _activatedRoute: ActivatedRoute, private _router: Router) {
    }

    onBackButtonClick(): void {
        this._router.navigate(['/examples']);

        ////Example using Relative Path
        //this._router.navigate(['examples'], { relativeTo; this.route });

        ////Example using Query Params
        //this._router.navigate(
        //    ['/examples'],
        //    { queryParams: { filterBy: 'filte', showImage: true } }
        //);

        ////using outlets for SECONDARY outlets
        //this._router.navigate([{ outlets: { secondaryExampleRouterOutlet: ['Another-Link-Parameters-Array-specifying-secondary-path'] } }]);
        ////CLEAR the outlet for SECONDARY outlets
        //this._router.navigate([{ outlets: { secondaryExampleRouterOutlet: ['null'] } }]);
    }

    onCancelButtonClick(): void {
        this.statusMessage = 'Request Cancelled'
        this.subscription.unsubscribe();
    }

    ngOnInit() {
        this._activatedRoute.params.subscribe(
            params => {
                let exCode = params['exampleId'];
                //let qp = this._activatedRoute.snapshot.queryParams['filterBy'] || ''; //OPTIONAL query parameter receiving, use this to BIND a local variable that is used in the HTML

                //may want to create ANOTHER method to seperate this logic from this .ts file
                this.subscription = this._exampleService.getExampleById(exCode)
                    .retryWhen((err) => {
                        return err.scan((retryCount) => {
                            retryCount += 1;
                            if (retryCount < 5) {
                                this.statusMessage = 'Retrying ..... #' + retryCount;
                                return retryCount;
                            } else {
                                throw (err);
                            }
                        }, 0).delay(1000)
                    })
                    .subscribe(
                    (exData) => {
                        if (exData == null) {
                            this.statusMessage = 'Example Does NOT Exist';
                        } else {
                            this.example = exData;
                            console.log('RECEIVED exDATA in Obesrvable-Params version');
                        }
                        this.example = exData
                    },
                    (error) => {
                        this.statusMessage = 'Problem with the Service. Please Retry after some time ... ';
                        console.log(error);
                    }
                    );
            }
        );
    }
}