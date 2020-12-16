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
		const compareOneCommand: Command = this.tryGetCommand('GLASSWALL_DOWNLOAD');
		if (compareOneCommand) {
			// This command should be hidden unless exactly one row is selected.
			compareOneCommand.visible = event.selectedRows.length > 0;
		}
		const defaultDownloadCommand: Command = this.tryGetCommand('downloadCommand');
		if (defaultDownloadCommand) {
			console.log("defaultDownloadCommand captured!");
			defaultDownloadCommand.visible = false;
		}
		const defaultDownloadCommand2: Command = this.tryGetCommand('Download');
		if (defaultDownloadCommand2) {
			console.log("defaultDownloadCommand2 captured!");
			defaultDownloadCommand2.visible = false;
		}
	}

	@override
	public onExecute(event: IListViewCommandSetExecuteEventParameters): void {
		switch (event.itemId) {
			case 'GLASSWALL_DOWNLOAD':
				// let fileName: string;
				let itemURLs: string[] = [];
				let itemNames: string = '';
				event.selectedRows.forEach(row => {
					itemNames = itemNames + row.getValueByName("FileLeafRef") + "\n";
					const itemId: string = row.getValueByName("UniqueId");
					itemURLs.push(`"https://graph.microsoft.com/v1.0/me/drive/items/${itemId.substring(1,itemId.length-1)}"`);
				});
				console.log(`You have selected following files: \n\n${itemNames}`);
				const encodedUrl: string = encodeURI(itemURLs.join(","));
				console.log("selected ItemURLs: " + encodedUrl);

				const uniqueId = event.selectedRows[0].getValueByName("UniqueId");
				console.log("Unique Id: " + uniqueId);
				// const strUniqueId: string = uniqueId.substring(1,uniqueId.length-1);
				// const graphUrl = encodeURI(`"https://graph.microsoft.com/v1.0/me/drive/items/${strUniqueId}"`);

				const cultureInfo: string = this.context.pageContext.cultureInfo.currentUICultureName.toLowerCase();
				const userEmail: string = encodeURIComponent(this.context.pageContext.user.email);
				
				const param: string = `cultureName=${cultureInfo}&client=OneDrive&userId=${userEmail}&items=%5B${encodedUrl}%5D`;
				const headers: any = {
					'Content-Type': "application/x-www-form-urlencoded"
				};
						  
				this._FileHandleService.executeRequest("post", "/filehandler/download", headers, param);

				// Dialog.alert(`You have selected following files: \n\n${items}`);
				//event.selectedRows[0].getValueByName("UniqueId")
				break;
			default:
				throw new Error('Unknown command');
		}
	}

	private openFile(data: any, fileName: string) {
		if (data == "" || data == undefined) {
			alert("Falied to open PDF.");
		} else { //For IE using atob convert base64 encoded data to byte array
			if (window.navigator && window.navigator.msSaveOrOpenBlob) {
				var byteCharacters = atob(data);
				var byteNumbers = new Array(byteCharacters.length);
				for (var i = 0; i < byteCharacters.length; i++) {
					byteNumbers[i] = byteCharacters.charCodeAt(i);
				}
				var byteArray = new Uint8Array(byteNumbers);
				var blob = new Blob([byteArray], {
					type: 'application/pdf'
				});
				window.navigator.msSaveOrOpenBlob(blob, fileName);
			} else { // Directly use base 64 encoded data for rest browsers (not IE)
				var base64EncodedPDF = data;
				var dataURI = "data:application/pdf;base64," + base64EncodedPDF;
				window.open(dataURI, '_blank');
			}
		
		}
	}

	private NavigateToFileHandler(items: string) {
		//cultureName=en-us
		//&client=SharePoint
		//&userId=rgregg@contoso.com
		//&items=https://graph.microsoft.com/v1.0/me/drive/items/4D679BEA-6F9B-4106-AB11-101DDE52B06E

	}
}
