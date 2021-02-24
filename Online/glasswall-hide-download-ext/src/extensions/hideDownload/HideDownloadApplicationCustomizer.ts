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
	executeFrequency: number;
}

/** A Custom Action which can be run during execution of a Client Side Application */
export default class HideDownloadApplicationCustomizer
	extends BaseApplicationCustomizer<IHideDownloadApplicationCustomizerProperties> {

	private _DownloadCommands: string[] = [];
	private _CommandBarClientId = "od-TopBar-item";
	private _ItemListClientId = "od-ItemContent-list";

	private _Observer: any;
	private _MutationConfig: any;

	private _isCommandbarObserverAttached = false;
	private _isItemListObserverAttached = false;
	private _isCommandbarEventAttached = false;
	private _isItemListEventAttached = false;

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
		console.log(`Initialized ${strings.Title}`);

		//Set Download Commands
		if (this.properties.commandName) {
			this._DownloadCommands = this.properties.commandName.split(",").map(c => c.trim());
		} else {
			this._DownloadCommands.push("download");
			this._DownloadCommands.push("downloadCommand");
		}

		// Associate observers and events.
		this.associateObserverAndEvents();

		// Associate observers and events on page navigations as well.
		this.context.application.navigatedEvent.add(this, () => {
			console.log('Navigated Event:', window.location.href);
			setTimeout(() => {
				this.associateObserverAndEvents();				
			}, 100);
		});

		// Additionally, if the download buttons are not hidden with event listners, we can hide them manually and retry every execute frequency interval (i.e. 300 ms).
		if (this.properties.executeFrequency && this.properties.executeFrequency >= 50) {
			setInterval(() => {
				this.hideDownloadButtons();
			}, this.properties.executeFrequency);	
		}

		return Promise.resolve();
	}

	public componentWillUnmount() {
		// Disconnect mutation observer
		this._Observer.disconnect();
	}

	// Callback function to execute when mutations are observed
	private mutationCallback() {
		this.TryHideDownloadButtons();
	}

	private associateObserverAndEvents() {

		// To hide Download button in Top Commandbar
		this.tryAssociateObserverOnCommandBar();
		// For fail safe - If Mutation observer doesn't work, let's attach change event to Commandbar
		this.tryAttachOnChangeEventToCommandBar();

		// To hide Download button in Context Menu
		this.tryAssociateObserverOnItemList();
		// For fail safe - If Mutation observer doesn't work, let's attach change event to Context Menu
		this.tryAttachOnChangeEventToItemList();		
	}

	private tryAssociateObserverOnCommandBar(retryCount: number = 1) {
		let commandBarItems = document.getElementsByClassName(this._CommandBarClientId);
		if (commandBarItems.length > 0) {
			this.associateObserverOnCommandBar(commandBarItems[0]);
		} else if (!this._isCommandbarObserverAttached && retryCount < 10) {
			//retry after 100ms to check if the element is loaded
			console.log("Commandbar Observer retryCount: " + retryCount);
			setTimeout(() => {
				this.tryAssociateObserverOnCommandBar(++retryCount);
			}, 100);
		}
	}

	private associateObserverOnCommandBar(commandBar: any) {
		const ATTR_GW_COMMANDBAR_OBSERVER: string = "xn_gw_cb_observer";

		if (commandBar && commandBar.getAttribute(ATTR_GW_COMMANDBAR_OBSERVER) !== "true") {
			try {
				this._Observer.observe(commandBar, this._MutationConfig);
				commandBar.setAttribute(ATTR_GW_COMMANDBAR_OBSERVER, "true");
				this._isCommandbarObserverAttached = true;
				console.log("Commandbar Observer attached successfully.");
			} catch (error) {
				console.log("An error occurred while associating observer for Command Bar. Message: " + error.message);
			}
		}
	}

	private tryAssociateObserverOnItemList(retryCount: number = 1) {
		let listItems = document.getElementsByClassName(this._ItemListClientId);
		if (listItems.length > 0) {
			this.associateObserverOnItemList(listItems[0]);
		} else if (!this._isItemListObserverAttached && retryCount < 10) {
			//retry after 100ms to check if the element is loaded
			console.log("ItemList Observer retryCount: " + retryCount);
			setTimeout(() => {
				this.tryAssociateObserverOnItemList(++retryCount);
			}, 100);
		}
	}

	private associateObserverOnItemList(itemList: any) {
		const ATTR_GW_ITEMLIST_OBSERVER: string = "xn_gw_itemlist_observer";

		if (itemList && itemList.getAttribute(ATTR_GW_ITEMLIST_OBSERVER) !== "true") {
			try {
				this._Observer.observe(itemList, this._MutationConfig);
				itemList.setAttribute(ATTR_GW_ITEMLIST_OBSERVER, "true");
				this._isItemListObserverAttached = true;
				console.log("ItemList Observer attached successfully.");
			} catch (error) {
				console.log("An error occurred while associating observer for Item List. Message: " + error.message);
			}
		}
	}

	private tryAttachOnChangeEventToCommandBar(retryCount: number = 1) {
		let commandBarItems = document.getElementsByClassName(this._CommandBarClientId);
		if (commandBarItems.length > 0) {
			this.attachOnChangeEventToCommandBar(commandBarItems[0]);
		} else if (!this._isCommandbarEventAttached && retryCount < 10) {
			//retry after 100ms to check if the element is loaded
			console.log("Commandbar Event retryCount: " + retryCount);
			setTimeout(() => {
				this.tryAttachOnChangeEventToCommandBar(++retryCount);
			}, 100);
		}
	}

	private attachOnChangeEventToCommandBar(commandBar: any) {
		const ATTR_GW_COMMANDBAR_EVENT: string = "xn_gw_cb_evt";

		if (commandBar && commandBar.getAttribute(ATTR_GW_COMMANDBAR_EVENT) !== "true") {
			try {
				commandBar.addEventListener("DOMNodeInserted", this.onTopCommandBarChanged.bind(this));
				commandBar.setAttribute(ATTR_GW_COMMANDBAR_EVENT, "true");
				this._isCommandbarEventAttached = true;
				console.log("Commandbar Event attached successfully.");
			} catch(error) {
				console.log("An error occurred while associating event for Command Bar. Message: " + error.message);
			}
		}
	}

	private tryAttachOnChangeEventToItemList(retryCount: number = 1) {
		let listItems = document.getElementsByClassName(this._ItemListClientId);
		if (listItems.length > 0) {
			this.attachOnChangeEventToItemList(listItems[0]);
		} else if (!this._isItemListEventAttached && retryCount < 10) {
			//retry after 100ms to check if the element is loaded
			console.log("ItemList Event retryCount: " + retryCount);
			setTimeout(() => {
				this.tryAttachOnChangeEventToItemList(++retryCount);
			}, 100);
		}
	}

	private attachOnChangeEventToItemList(itemList: any) {
		const ATTR_GW_ITEMLIST_EVENT: string = "xn_gw_itemlist_evt";

		if (itemList && itemList.getAttribute(ATTR_GW_ITEMLIST_EVENT) !== "true") {
			try {
				itemList.addEventListener("DOMNodeInserted", this.onItemContentListChanged.bind(this));
				itemList.setAttribute(ATTR_GW_ITEMLIST_EVENT, "true");
				this._isItemListEventAttached = true;
				console.log("ItemList Event attached successfully.");
			} catch(error) {
				console.log("An error occurred while associating event for Item List. Message: " + error.message);
			}
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
				if (this._DownloadCommands.indexOf(xn_commandname) >= 0) {
					btn.setAttribute(ATTR_GW_DOWNLOAD_BUTTON, "true");
					btn.style.display = "none";
				}
			}
		});
	}
}
