import { IFileHandlerService } from "./IFileHandlerService";
import { ServiceKey, ServiceScope } from "@microsoft/sp-core-library";
import { AadHttpClient, IHttpClientOptions, AadHttpClientFactory } from '@microsoft/sp-http';

export class GlasswallFileHandlerService implements IFileHandlerService {
    
    public static readonly serviceKey: ServiceKey<IFileHandlerService> = ServiceKey.create<IFileHandlerService>("XN.GW.GlasswallFileHandlerService", GlasswallFileHandlerService);

    private _appUri: string = "api://glasswallnetfw";
	public set appUri(value: string) { this._appUri = value; }
	public get appUri(): string { return this._appUri; }

    private _apiUrl: string = "https://app-gwfhnetfw-dev.azurewebsites.net";
	public set apiUrl(value: string) { this._apiUrl = value; }
	public get apiUrl(): string { return this._apiUrl; }

    private _AadHttpClientFactory: AadHttpClientFactory;

    constructor(serviceScope: ServiceScope) {
        serviceScope.whenFinished(() => {

            console.log("initializing AAD Client...");
            this._AadHttpClientFactory = serviceScope.consume(AadHttpClientFactory.serviceKey);
		});
    }

    public rebuildFile(parameters: string): Promise<any> {

        const headers: any = {
            'Content-Type': "application/x-www-form-urlencoded"
        };

        let targetUrl = `${this._apiUrl}/filehandler/download`;
        let options: IHttpClientOptions = {
            method: "post",
            headers: new Headers(headers),
            body: parameters,
            credentials: 'same-origin',
            mode: 'no-cors'
        };

        return this._AadHttpClientFactory.getClient(this._appUri).then((aadClient: AadHttpClient) => {
            return aadClient.post(targetUrl, AadHttpClient.configurations.v1, options);
        });
    }
}