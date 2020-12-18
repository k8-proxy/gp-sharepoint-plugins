import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
	BaseListViewCommandSet,
	Command,
	IListViewCommandSetListViewUpdatedParameters,
	IListViewCommandSetExecuteEventParameters
} from '@microsoft/sp-listview-extensibility';
import { Dialog } from '@microsoft/sp-dialog';

import * as strings from 'GlasswallDownloadCommandSetStrings';
import { IFileHandlerService } from '../../services/FileHandlerService/IFileHandlerService';
import { FileHandlerService } from '../../services/FileHandlerService/FileHandlerService';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IGlasswallDownloadCommandSetProperties {
	// This is an example; replace with your own properties
	commandName: string;
}

const LOG_SOURCE: string = 'GlasswallDownloadCommandSet';

export default class GlasswallDownloadCommandSet extends BaseListViewCommandSet<IGlasswallDownloadCommandSetProperties> {

	private _FileHandleService: IFileHandlerService;

	@override
	public onInit(): Promise<void> {
		Log.info(LOG_SOURCE, 'Initialized GlasswallDownloadCommandSet');

		this._FileHandleService = this.context.serviceScope.consume(FileHandlerService.serviceKey);

		return Promise.resolve();
	}

	@override
	public onListViewUpdated(event: IListViewCommandSetListViewUpdatedParameters): void {
		const gwDownloadCommand: Command = this.tryGetCommand('GLASSWALL_DOWNLOAD');
		if (gwDownloadCommand) {
			// This command should be hidden unless exactly one row is selected.
			gwDownloadCommand.visible = event.selectedRows.length > 0;
		}

		// Following code is just to test if we can capture default download command in the extension itself
		// const defaultDownloadCommand: Command = this.tryGetCommand('downloadCommand');
		// if (defaultDownloadCommand) {
		// 	console.log("defaultDownloadCommand captured!");
		// 	defaultDownloadCommand.visible = false;
		// }
		// const defaultDownloadCommand2: Command = this.tryGetCommand('Download');
		// if (defaultDownloadCommand2) {
		// 	console.log("defaultDownloadCommand2 captured!");
		// 	defaultDownloadCommand2.visible = false;
		// }
	}

	@override
	public onExecute(event: IListViewCommandSetExecuteEventParameters): void {
		switch (event.itemId) {
			case 'GLASSWALL_DOWNLOAD':
				let itemURLs: string[] = [];
				let itemNames: string = '';
				event.selectedRows.forEach(row => {
					itemNames = itemNames + row.getValueByName("FileLeafRef") + "\n";
					// const itemId: string = row.getValueByName("UniqueId");
					const itemUrl: string = row.getValueByName(".spItemUrl");
					//"https://xamariners.sharepoint.com:443/_api/v2.0/drives/b!v6JPv0Rb102Wn66JRVNB3pOyg8INDCtEgcAxJ5p4Xe3mMP-1M7gTR4VwfglbUQtx/items/01KK7SSJDKKPGVOAFR4JAIJOYHRZYCUB22?version=Published"
					const graphUrl: string = this.buildGraphUrl(itemUrl);
					console.log(graphUrl);
					itemURLs.push(graphUrl);
				});

				console.log(`You have selected following files: \n\n${itemNames}`);
				const encodedUrl: string = encodeURI(itemURLs.join(","));
				console.log("selected ItemURLs: " + encodedUrl);

				const cultureInfo: string = this.context.pageContext.cultureInfo.currentUICultureName.toLowerCase();
				const userEmail: string = encodeURIComponent(this.context.pageContext.user.email);
				
				const param: string = `cultureName=${cultureInfo}&client=OneDrive&userId=${userEmail}&items=%5B${encodedUrl}%5D`;
				const headers: any = {
					'Content-Type': "application/x-www-form-urlencoded"
				};
						  
				this._FileHandleService.executeRequest("post", "/filehandler/download", headers, param);

				// Dialog.alert(`You have selected following files: \n\n${items}`);
				break;
			default:
				throw new Error('Unknown command');
		}
	}

	private buildGraphUrl(itemUrl: string): string {
		const itemUri: URL = new URL(itemUrl);
		const drivePath: string = itemUri.pathname.substring(itemUri.pathname.indexOf("/drives/"));
		const graphUrl: string = `https://graph.microsoft.com/v1.0${drivePath}`;
		return graphUrl;
	}
}
