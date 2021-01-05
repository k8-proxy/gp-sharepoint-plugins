import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
	BaseListViewCommandSet,
	Command,
	IListViewCommandSetListViewUpdatedParameters,
	IListViewCommandSetExecuteEventParameters
} from '@microsoft/sp-listview-extensibility';

import { IFileHandlerService } from '../../services/FileHandlerService/IFileHandlerService';
import { GlasswallFileHandlerService } from '../../services/FileHandlerService/FileHandlerService';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IGlasswallDownloadCommandSetProperties {
	apiUrl: string;
	appUri: string;
}

const LOG_SOURCE: string = 'GlasswallDownloadCommandSet';
const GLASSWALL_DOWNLOAD_COMMAND_NAME: string = 'GLASSWALL_DOWNLOAD';

export default class GlasswallDownloadCommandSet extends BaseListViewCommandSet<IGlasswallDownloadCommandSetProperties> {

	private _GlasswallFileHandlerService: IFileHandlerService;

	@override
	public onInit(): Promise<void> {
		Log.info(LOG_SOURCE, 'Initialized GlasswallDownloadCommandSet');

		this._GlasswallFileHandlerService = this.context.serviceScope.consume(GlasswallFileHandlerService.serviceKey);
		if (this.properties.apiUrl != null) {
			this._GlasswallFileHandlerService.apiUrl = this.properties.apiUrl;
		}
		if (this.properties.appUri != null) {
			this._GlasswallFileHandlerService.appUri = this.properties.appUri;
		}

		return Promise.resolve();
	}

	@override
	public onListViewUpdated(event: IListViewCommandSetListViewUpdatedParameters): void {
		const gwDownloadCommand: Command = this.tryGetCommand(GLASSWALL_DOWNLOAD_COMMAND_NAME);
		if (gwDownloadCommand) {
			gwDownloadCommand.visible = event.selectedRows.length > 0;
		}
	}

	@override
	public onExecute(event: IListViewCommandSetExecuteEventParameters): void {
		switch (event.itemId) {
			case GLASSWALL_DOWNLOAD_COMMAND_NAME:
				let itemURLs: string[] = [];
				event.selectedRows.forEach(row => itemURLs.push(this.buildGraphUrl(row.getValueByName(".spItemUrl"))));
				
				const param: string = this.prepareHandlerParameters(itemURLs);
				this._GlasswallFileHandlerService.rebuildFile(param).then(response => {
					//Successful download
				}).catch(error => {
					throw new Error(error);
				});
				break;
			default:
				throw new Error('Unknown command');
		}
	}

	private buildGraphUrl(itemUrl: string) {
		const itemUri: URL = new URL(itemUrl);
		const drivePath: string = itemUri.pathname.substring(itemUri.pathname.indexOf("/drives/"));
		const graphUrl: string = `https://graph.microsoft.com/v1.0${drivePath}`;
		console.log(graphUrl);
		return graphUrl;
	}

	private prepareHandlerParameters(itemURLs: string[]) {
		const encodedUrl: string = encodeURI(`[${itemURLs.join(",")}]`);
		const cultureInfo: string = this.context.pageContext.cultureInfo.currentUICultureName.toLowerCase();
		const userEmail: string = encodeURIComponent(this.context.pageContext.user.email);

		return `cultureName=${cultureInfo}&client=OneDrive&userId=${userEmail}&items=${encodedUrl}`;
	}
}
