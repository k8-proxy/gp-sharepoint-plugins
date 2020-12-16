import { IFileHandlerService } from "./IFileHandlerService";
import { ServiceKey, ServiceScope } from "@microsoft/sp-core-library";
import { PageContext } from "@microsoft/sp-page-context";
import { AadHttpClient, HttpClientResponse, IHttpClientOptions, AadHttpClientFactory } from '@microsoft/sp-http';

export class FileHandlerService implements IFileHandlerService {
    
    public static readonly serviceKey: ServiceKey<IFileHandlerService> = ServiceKey.create<IFileHandlerService>("XN.GW.IFileHandlerService", FileHandlerService);

    private _apiUrl: string = "https://app-gwfhnetfw-dev.azurewebsites.net";
    private _apiMethodUrl: string = "/filehandler/download";

    // private _PageContext: PageContext;
    private _AadClient: AadHttpClient;
    private _AadHttpClientFactory: AadHttpClientFactory;

    constructor(serviceScope: ServiceScope) {
        serviceScope.whenFinished(() => {

            console.log("initializing AAD Client...");
            this._AadHttpClientFactory = serviceScope.consume(AadHttpClientFactory.serviceKey);
            // this._AadHttpClientFactory.getClient(this._apiUrl)
            this._AadHttpClientFactory.getClient("56808e55-354e-48fc-ad57-7c74a88757c4")
            .then((client: AadHttpClient) => {
                this._AadClient = client;
            });

			// this._PageContext = serviceScope.consume(PageContext.serviceKey);

			// sp.setup({
			// 	spfxContext: {
			// 		pageContext: this._PageContext
			// 	}
			// });
		});
    }

    public executeRequest(method: string, endpointUrl: string, headers: any, parameters: string): Promise<any> {

        console.log("calling file-hanlder API...");
        console.log("headers: " + JSON.stringify(headers));
        console.log("body: " + parameters);

        let targetUrl = this._apiUrl + endpointUrl;
        let options: IHttpClientOptions = {
            method: method,
            headers: new Headers(headers),
            body: parameters,
            credentials: 'same-origin',
            mode: 'no-cors'
          };
        return this._AadClient.post(targetUrl, AadHttpClient.configurations.v1, options);
    }
}