export interface HttpRequest {
    method: string;
    url: string;
    contentType?: string;
    content?: any;
}
export interface Liveness {
    livenessName: string;
    onStateFrom: string;
    lastExecuted: string;
    status: string;
    livenessResult: string;
    checks : Array<Check>;
}

export interface Check {
    name: string;
    message: string;
    milliSeconds: number;
    run: boolean;
    path: string,
    isHealthy: boolean
}

interface WebHook {
    name: string;
    uri: string;
    payload: string;
}