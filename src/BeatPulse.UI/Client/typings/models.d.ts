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
// TODO -> This properties will be camelCase in beatpulse 2.0
export interface Check {
    Name: string;
    Message: string;
    MilliSeconds: number;
    Run: boolean;
    Path: string,
    IsHealthy: boolean
}

interface WebHook {
    name: string;
    uri: string;
    payload: string;
}