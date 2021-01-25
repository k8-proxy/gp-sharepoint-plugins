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
	commandName: string;
}

/** A Custom Action which can be run during execution of a Client Side Application */
export default class HideDownloadApplicationCustomizer
	extends BaseApplicationCustomizer<IHideDownloadApplicationCustomizerProperties> {

	private _DOWNLOAD_COMMAND = "downloadCommand";
	private _CommandBarClientId = "od-TopBar-item";
	private _ItemListClientId = "od-ItemContent-list";

	private _Observer: any;
	private _MutationConfig: any;

	constructor() {
		super();

		// Options for the observer (which mutations to observe)
		this._MutationConfig = { attributes: false, childList: true, subtree: true };

		// Create an observer instance linked to the callback function
		this._Observer = new MutationObserver(this.mutationCallback.bind(this));
	}

	@override
	public onInit(): Promise<void> {
		Log.info(LOG_SOURCE, `Initialized ${strings.Title}`);

		// To hide Download button in Top Commandbar
		this.tryAssociateObserverOnCommandBar();
		// For fail safe - If Mutation observer doesn't work
		this.tryAttachOnChangeEventToCommandBar();

		// To hide Download button in Context Menu
		this.tryAssociateObserverOnItemList();
		this.tryAttachOnChangeEventToItemList();

		// Additionally, if the download buttons are not hidden with event listners, we can hide them manually and retry every 500 ms.
		// setInterval(() => {
		// 	this.hideDownloadButtons(true);
		// }, 500);

		return Promise.resolve();
	}

	protected componentWillUnmount() {
		// Disconnect mutation observer
		this._Observer.disconnect();
	}

	// Callback function to execute when mutations are observed
	private mutationCallback() {
		this.TryHideDownloadButtons();
	}

	private tryAssociateObserverOnCommandBar(retryCount: number = 1) {
		let commandBarItems = document.getElementsByClassName(this._CommandBarClientId);
		if (commandBarItems.length > 0) {
			this.associateObserverOnCommandBar(commandBarItems[0]);
		} else if (retryCount < 3) {
			//retry after 100ms to check if the element is loaded
			setTimeout(() => {
				this.tryAssociateObserverOnCommandBar(++retryCount);
			}, 100);
		}
	}

	private associateObserverOnCommandBar(commandBar: any) {
		const ATTR_GW_COMMANDBAR_OBSERVER: string = "xn_gw_cb_observer";

		if (commandBar.getAttribute(ATTR_GW_COMMANDBAR_OBSERVER) !== "true") {
			try {
				this._Observer.observe(commandBar, this._MutationConfig);
				commandBar.setAttribute(ATTR_GW_COMMANDBAR_OBSERVER, "true");
			} catch (error) {
				console.log("An error occurred while associating observer for Command Bar. Message: " + error.message);
			}
		}
	}

	private tryAssociateObserverOnItemList(retryCount: number = 1) {
		let listItems = document.getElementsByClassName(this._ItemListClientId);
		if (listItems.length > 0) {
			this.associateObserverOnItemList(listItems[0]);
		} else if (retryCount < 3) {
			//retry after 100ms to check if the element is loaded
			setTimeout(() => {
				this.tryAssociateObserverOnItemList(++retryCount);
			}, 100);
		}
	}

	private associateObserverOnItemList(itemList: any) {
		const ATTR_GW_ITEMLIST_OBSERVER: string = "xn_gw_itemlist_observer";

		if (itemList.getAttribute(ATTR_GW_ITEMLIST_OBSERVER) !== "true") {
			try {
				this._Observer.observe(itemList, this._MutationConfig);
				itemList.setAttribute(ATTR_GW_ITEMLIST_OBSERVER, "true");
			} catch (error) {
				console.log("An error occurred while associating observer for Item List. Message: " + error.message);
			}
		}
	}

	private tryAttachOnChangeEventToCommandBar(retryCount: number = 1) {
		let commandBarItems = document.getElementsByClassName(this._CommandBarClientId);
		if (commandBarItems.length > 0) {
			this.attachOnChangeEventToCommandBar(commandBarItems[0], retryCount);
		} else if (retryCount < 3) {
			//retry after 100ms to check if the element is loaded
			setTimeout(() => {
				this.attachOnChangeEventToCommandBar(++retryCount);
			}, 100);
		}
	}

	private attachOnChangeEventToCommandBar(commandBar: any, retryCount: number = 1) {
		const ATTR_GW_COMMANDBAR_EVENT: string = "xn_gw_cb_evt";

		if (commandBar.getAttribute(ATTR_GW_COMMANDBAR_EVENT) !== "true") {
			commandBar.addEventListener("DOMNodeInserted", this.onTopCommandBarChanged.bind(this));
			commandBar.setAttribute(ATTR_GW_COMMANDBAR_EVENT, "true");
		}
	}

	private tryAttachOnChangeEventToItemList(retryCount: number = 1) {
		let listItems = document.getElementsByClassName(this._ItemListClientId);
		if (listItems.length > 0) {
			this.attachOnChangeEventToItemList(listItems[0], retryCount);
		} else if (retryCount < 3) {
			//retry after 100ms to check if the element is loaded
			setTimeout(() => {
				this.attachOnChangeEventToItemList(++retryCount);
			}, 100);
		}
	}

	private attachOnChangeEventToItemList(itemList: any, retryCount: number = 1) {
		const ATTR_GW_ITEMLIST_EVENT: string = "xn_gw_itemlist_evt";

		if (itemList.getAttribute(ATTR_GW_ITEMLIST_EVENT) !== "true") {
			itemList.addEventListener("DOMNodeInserted", this.onItemContentListChanged.bind(this));
			itemList.setAttribute(ATTR_GW_ITEMLIST_EVENT, "true");
		}
	}

	private onTopCommandBarChanged() {
		this.TryHideDownloadButtons();
	}

	private onItemContentListChanged() {
		this.TryHideDownloadButtons();
	}

	private TryHideDownloadButtons(retryCount: number = 0) {
		this.hideDownloadButtons();
		if (retryCount < 5) {
			//retry after 10ms to hide download button if it is not hidden in previous attempt
			setTimeout(() => {
				this.TryHideDownloadButtons(++retryCount);
			}, 10);
		}
	}

	private hideDownloadButtons() {
		const ATTR_GW_DOWNLOAD_BUTTON: string = "xn_gw_download_btn";

		let downloadButtons = document.getElementsByName("Download");
		downloadButtons.forEach(btn => {
			let xn_glasswall_attr = btn.getAttribute(ATTR_GW_DOWNLOAD_BUTTON);
			if (!xn_glasswall_attr) {
				let xn_commandname = btn.getAttribute("data-automationid");
				if (xn_commandname == this._DOWNLOAD_COMMAND) {
					btn.setAttribute(ATTR_GW_DOWNLOAD_BUTTON, "true");
					btn.style.display = "none";
				}
			}
		});
	}
}
