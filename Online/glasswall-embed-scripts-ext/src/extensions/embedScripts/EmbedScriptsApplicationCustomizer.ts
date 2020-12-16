import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
	BaseApplicationCustomizer
} from '@microsoft/sp-application-base';
import { Dialog } from '@microsoft/sp-dialog';

import * as strings from 'EmbedScriptsApplicationCustomizerStrings';

const LOG_SOURCE: string = 'EmbedScriptsApplicationCustomizer';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IEmbedScriptsApplicationCustomizerProperties {
	// This is an example; replace with your own property
	testMessage: string;
}

/** A Custom Action which can be run during execution of a Client Side Application */
export default class EmbedScriptsApplicationCustomizer
	extends BaseApplicationCustomizer<IEmbedScriptsApplicationCustomizerProperties> {

	private _jQueryUrl: string = "https://code.jquery.com/jquery-3.5.1.min.js";
	// private _jQueryUIUrl: string = "https://xamariners.sharepoint.com/SiteAssets/scripts/jQuery/jquery-ui.min.js";
	private _externalJsUrl: string = "https://xamariners.sharepoint.com/SiteAssets/Glasswall/glasswall-download-customizer.js";

	@override
	public onInit(): Promise<void> {
		Log.info(LOG_SOURCE, `Initialized ${strings.Title}`);

		let scriptElement: any = document.createElement("script");
		scriptElement.src = this._jQueryUrl;
		scriptElement.type = "text/javascript";

		//Load the custom script file only after jQuery is loaded.
		// Attach handlers for all browsers
		var done = false;
		scriptElement.onload = scriptElement.onreadystatechange = () => {
			if (!done && (!scriptElement.readyState
				|| scriptElement.readyState == "loaded"
				|| scriptElement.readyState == "complete")) {
				done = true;

				// Load custom script file
				this.loadCustomScripts();

				// Handle memory leak in IE
				scriptElement.onload = scriptElement.onreadystatechange = null;
				document.getElementsByTagName("head")[0].removeChild(scriptElement);
			}
		};

		document.getElementsByTagName("head")[0].appendChild(scriptElement);

		// let jQueryUIScriptTag: HTMLScriptElement = document.createElement("script");
		// jQueryUIScriptTag.src = this._jQueryUIUrl;
		// jQueryUIScriptTag.type = "text/javascript";
		// document.getElementsByTagName("head")[0].appendChild(jQueryUIScriptTag);

		return Promise.resolve();
	}

	private loadCustomScripts() {
		let revision: string = (new Date()).toISOString().substring(0, 16).replace(/[^0-9]/g, "");
		let scriptElement: HTMLScriptElement = document.createElement("script");
		scriptElement.src = this._externalJsUrl + "?rev=" + revision;
		scriptElement.type = "text/javascript";
		document.getElementsByTagName("head")[0].appendChild(scriptElement);

		console.log(`DownloadCustomizerApplicationCustomizer.onInit(): Added script link.`);
	}
}
