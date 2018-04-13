
class BeatPulseClient {
    constructor(endpoint) {
        this.endpoint = endpoint;
        this.http = new HttpClient();
        this._pollingInterval = undefined;
    }
    healthCheck() {
        return this.http.sendRequest({
            url: this.endpoint,
            method: "get"
        });
    }

    startPolling(interval, onTimeElapsedCallback) {
        this._pollingInterval && clearInterval(this._pollingInterval);
        this._pollingInterval = setInterval(onTimeElapsedCallback, interval);
    }
}

class HttpClient {
    sendRequest(request) {
        return new Promise((resolve, reject) => {
            const xhr = new XMLHttpRequest();
            xhr.open(request.method, request.url, true);

            xhr.setRequestHeader("X-Request-Client", "XMLHttpClient");
            xhr.setRequestHeader("Content-type", request.contentType || "application/json");

            xhr.onload = () => {
                if (xhr.status >= 200 && xhr.status < 300) {
                    resolve(new HttpResponse(xhr));
                } else {
                    reject(new HttpError(xhr));
                }
            }
            xhr.onerror = () => {
                reject(new HttpError(xhr));
            }

            xhr.ontimeout = () => {
                reject(new HttpError(xhr));
            }
            xhr.send(request.content || "");
        });
    }
}

class HttpError {
    constructor(xhrError) {
        var {response, status, statusCode} =  xhrError;
        this.response = response;
        this.status = status;
        this.statusCode = statusCode;
    }
}

class HttpResponse {
    constructor(xhrResponse) {
        const {status, statusText, response, responseText} =  xhrResponse;
        this.status = status;
        this.statusText = statusText;
        this.response = response || responseText;
        this.data = JSON.parse(this.response);
    }
}

(function (BeatPulse) {

    BeatPulse.client = new BeatPulseClient(window.location.origin + uiEndpoint);
    BeatPulse.healthStatus = {
        up: "Up",
        down: "Down",
        degraded: "Degraded"
    };
    if (!("BeatPulse" in window)) {
        window.BeatPulse = BeatPulse;
    }

})(window.BeatPulse || {});



