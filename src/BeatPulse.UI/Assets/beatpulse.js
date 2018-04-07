(function (BeatPulse) {

    //Temporal code and url mapping

    BeatPulse.client = BeatPulse.client || {};
    BeatPulse.client._request = sendRequest;
    BeatPulse.client.checkHealth = () => {
        return BeatPulse.client._request({
            url: window.location.origin + "/beatpulse-api",
            method: "get"
        });
    }

    function HttpError(xhrError) {
        return {
            response: xhrError.response,
            status: xhrError.status,
            statusCode: xhrError.statusCode
        }
    }

    function HttpResponse(xhrResponse) {
        let response = xhrResponse.response || xhrResponse.responseText;
        return {
            status: xhrResponse.status,
            statusText: xhrResponse.statusText,
            response,
            data: JSON.parse(response)
        }
    }

    function sendRequest(request) {
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

    if (!("BeatPulse" in window)) {
        window.BeatPulse = BeatPulse;
    }

})
(window.BeatPulse || {});



