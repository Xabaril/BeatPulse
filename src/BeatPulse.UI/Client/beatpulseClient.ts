import { HttpClient } from "./http/httpClient";

export class BeatPulseClient {
    private _http : HttpClient;
    private _pollingInterval : Nullable<number>;

    constructor(private endpoint: string) {
        this.endpoint = endpoint;
        this._http = new HttpClient();
        this._pollingInterval = null;
    }
    healthCheck() : Promise<any> {
        return this._http.sendRequest({
            url: this.endpoint,
            method: "get"
        });
    }

    startPolling(interval : string | number, onTimeElapsedCallback: () => void) {
        this._pollingInterval && clearInterval(this._pollingInterval);
        this._pollingInterval = setInterval(onTimeElapsedCallback, interval);
    }
}