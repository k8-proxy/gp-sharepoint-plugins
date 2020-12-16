import { override } from '@microsoft/decorators';
import { Log } from '@microsoft/sp-core-library';
import {
	BaseApplicationCustomizer
} from '@microsoft/sp-application-base';
// import { Dialog } from '@microsoft/sp-dialog';

import * as strings from 'HideDownloadApplicationCustomizerStrings';

const LOG_SOURCE: string = 'HideDownloadApplicationCustomizer';

/**
 * If your command set uses the ClientSideComponentProperties JSON input,
 * it will be deserialized into the BaseExtension.properties object.
 * You can define an interface to describe it.
 */
export interface IHideDownloadApplicationCustomizerProperties {
	// This is an example; replace with your own property
	commandName: string;
}

/** A Custom Action which can be run during execution of a Client Side Application */
export default class HideDownloadApplicationCustomizer
	extends BaseApplicationCustomizer<IHideDownloadApplicationCustomizerProperties> {

	@override
	public onInit(): Promise<void> {
		Log.info(LOG_SOURCE, `Initialized ${strings.Title}`);

		// To hide Download button in Top Commandbar
		let topBarItems = document.getElementsByClassName("od-TopBar-item");
		if (topBarItems.length > 0) {
			topBarItems[0].addEventListener("DOMNodeInserted", this.onTopbarChanged.bind(this));
		} else {
			setTimeout(() => {
				if (topBarItems.length > 0) {
					topBarItems[0].addEventListener("DOMNodeInserted", this.onTopbarChanged.bind(this));
				}
			}, 300);
		}

		// //For context menu fix
		// let bodyItems = document.getElementsByTagName("body");
		// bodyItems[0].addEventListener('DOMNodeInserted', (e) => {
		// 	let element: any = e.target;
		// 	if (element.class.indexOf('ms-ContextualMenu-Callout') >= 0) {
		// 		console.log('context menu dom node inserted');
		// 		setTimeout(() => {
		// 			this.hideDownloadButtons();
		// 		}, 500);
		// 	}
		// });
		
		//od-ItemContent-list
		// To hide Download button in Context Menu
		let listItems = document.getElementsByClassName("od-ItemContent-list");
		if (listItems.length > 0) {
			listItems[0].addEventListener("DOMNodeInserted", this.onItemContentListChanged.bind(this));
		} else {
			setTimeout(() => {
				if (listItems.length > 0) {
					listItems[0].addEventListener("DOMNodeInserted", this.onItemContentListChanged.bind(this));
				}
			}, 300);
		}

		setInterval(() => {
			this.hideDownloadButtons();
		}, 500);

		return Promise.resolve();
	}
	private onTopbarChanged() {
		this.hideDownloadButtons();
	}

	private onItemContentListChanged() {
		console.log('od-ItemContent-list dom node inserted...');
		this.hideDownloadButtons();
	}

	private hideDownloadButtons() {
		let downloadButtons = document.getElementsByName("Download");
		downloadButtons.forEach(btn => {
			let xn_glasswall_attr = btn.getAttribute("xn_glasswall");
			if (!xn_glasswall_attr) {
				let xn_commandname = btn.getAttribute("data-automationid");
				if (xn_commandname == "downloadCommand") {
					btn.setAttribute("xn_glasswall_attr", "true");
					btn.style.display = "none";
				}
			}
		});
	}
}
