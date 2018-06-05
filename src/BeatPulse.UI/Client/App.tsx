import * as React from "react";
import { BeatPulseClient } from "./beatpulseClient";
import moment from "moment";
import { getStatusImage, statusUp, statusDown, statusDegraded } from "./beatpulseResources";
import { Liveness } from "./typings/models";

const beatPulseIntervalStorageKey = "beatpulse-ui-polling";

interface AppState {
    error: Nullable<string>;
    pollingIntervalSetting: string | number;
    livenessData : Array<Liveness>
}

interface AppProps {
    endpoint : string
}

export class App extends React.Component<AppProps, AppState> {
    private _beatpulseClient : BeatPulseClient;

    constructor(props: AppProps) {
        super(props);
        this._beatpulseClient = new BeatPulseClient(this.props.endpoint);
        this.initPolling = this.initPolling.bind(this);
        this.onPollinIntervalChange = this.onPollinIntervalChange.bind(this);
        
        const pollingIntervalSetting = localStorage.getItem(beatPulseIntervalStorageKey) || 10;

        this.state = {
            error: '',
            livenessData: [],
            pollingIntervalSetting
        }
    }

    componentDidMount() {
        this.load();
        this.initPolling();
    }

    async load() {
        try {

            let response = await this._beatpulseClient.healthCheck();
            for (let item of response.data as Array<Liveness>) {                
                item.onStateFrom = `${item.status} ${moment.utc(item.onStateFrom).fromNow()}`;
            }
            this.setState({
                livenessData: response.data
            });
        }
        catch (error) {
            this.setState({
                error: JSON.stringify(error)
            });
            console.error(error);
        };
    }

    initPolling() {
        localStorage.setItem(beatPulseIntervalStorageKey, this.state.pollingIntervalSetting.toString());
        this._beatpulseClient.startPolling(this.configuredInterval(), this.onPollingElapsed.bind(this));        
    }

    configuredInterval() : string | number {
        let configuredInterval = localStorage.getItem(beatPulseIntervalStorageKey) || this.state.pollingIntervalSetting;
        return (configuredInterval as any) * 1000;
    }

    onPollingElapsed() {
        this.setState({ error: '' });
        this.load();
    }

    onPollinIntervalChange(event: any){
        this.setState({
            pollingIntervalSetting: event.target.value
        })
    }

    getStatusPic(status: string) {
        return getStatusImage(status);
    }

    renderBackground(status: string) {
        return status === statusUp ? "" : status === statusDown ? "table-danger" : "table-warning";
    }

    formatDate(date: string) {
        return new Date(date).toLocaleString();
    }

    render() {
        return <React.Fragment>
            <div className="container">
                <div className="row">
                    <div className="col">
                        <h2 className="title">BeatPulse Liveness status</h2>
                    </div>
                    <div className="col text-right">
                        <label>Refresh every</label>
                        <input value={this.state.pollingIntervalSetting} onChange={this.onPollinIntervalChange} type="number" data-oninput="validity.valid && value > 0 ||(value=10)" className="polling-input" />
                        <label>seconds</label>
                        <button onClick={this.initPolling} type="button" className="btn btn-info btn-sm">Change</button>
                    </div>
                </div>
            </div>
            <div className="container">
                <div className="row">
                    <div className="table-responsive">
                        <table className="table">
                            <thead className="thead-dark">
                                <tr>
                                    <th>Name</th>
                                    <th>IsHealthy</th>
                                    <th>Status</th>
                                    <th>Last Execution</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this.state.livenessData.map((item, index) => {
                                    return <tr key={index} className={this.renderBackground(item.status)}>
                                        <td>
                                            {item.livenessName}
                                        </td>
                                        <td className="centered">
                                            <img className="status-icon" src={this.getStatusPic(item.status)} />
                                        </td>
                                        <td>
                                            {item.onStateFrom}
                                        </td>
                                        <td>
                                            {this.formatDate(item.lastExecuted)}
                                        </td>
                                    </tr>
                                })}
                            </tbody>
                        </table>
                    </div>
                    {this.state.error ?
                        <div className="w-100 alert alert-danger" role="alert">{this.state.error}</div>
                        : null
                    }                    
                </div>
            </div >
        </React.Fragment>
    }
}