import * as React from "react";
import { BeatPulseClient } from "../beatpulseClient";
import moment from "moment";
import { Liveness } from "../typings/models";
import { LivenessTable } from "./LivenessTable";
const HeartO = require("../../Assets/svg/heart-o.svg");

const beatPulseIntervalStorageKey = "beatpulse-ui-polling";

interface LivenessState {
    error: Nullable<string>;
    pollingIntervalSetting: string | number;
    livenessData: Array<Liveness>;
}

interface LivenessProps {
    endpoint: string
}

export class LivenessPage extends React.Component<LivenessProps, LivenessState> {
    private _beatpulseClient: BeatPulseClient;

    constructor(props: LivenessProps) {
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

            let livenessCollection = (await this._beatpulseClient.getData()).data as Array<Liveness>;
            for (let item of livenessCollection) {
                item.onStateFrom = `${item.status} ${moment.utc(item.onStateFrom).fromNow()}`;
            }

            if (livenessCollection && livenessCollection.length > 0) {
                this.setState({
                    livenessData: livenessCollection
                });
            }            
        }
        catch (error) {
            this.setState({
                error: 'Could not retrieve liveness data'
            });
            console.error(error);
        };
    }

    initPolling() {
        localStorage.setItem(beatPulseIntervalStorageKey, this.state.pollingIntervalSetting.toString());
        this._beatpulseClient.startPolling(this.configuredInterval(), this.onPollingElapsed.bind(this));
    }

    configuredInterval(): string | number {
        let configuredInterval = localStorage.getItem(beatPulseIntervalStorageKey) || this.state.pollingIntervalSetting;
        return (configuredInterval as any) * 1000;
    }

    onPollingElapsed() {
        this.setState({ error: '' });
        this.load();
    }

    onPollinIntervalChange(event: any) {
        this.setState({
            pollingIntervalSetting: event.target.value
        })
    }

    componentWillUnmount(){
        this._beatpulseClient.stopPolling();
    }

    render() {
        return <div id="wrapper" style={{ height: '100%', overflow: 'auto' }}>
                <div className="container">
                    <div className="row">
                        <div className="header-logo">
                            <img src={HeartO} className="logo-icon"/><h2 className="title">BeatPulse Liveness status</h2>
                        </div>
                        <div className="col text-right">
                            <label>Refresh every</label>
                            <input value={this.state.pollingIntervalSetting} onChange={this.onPollinIntervalChange} type="number" data-oninput="validity.valid && value > 0 ||(value=10)" className="polling-input" />
                            <label>seconds</label>
                            <button onClick={this.initPolling} type="button" className="btn btn-light btn-sm">Change</button>
                        </div>
                    </div>
                </div>
                <div className="container">
                    <div className="row">
                        <LivenessTable livenessData={this.state.livenessData}/>
                        {this.state.error ?
                            <div className="w-100 alert alert-danger" role="alert">{this.state.error}</div>
                            : null
                        }
                    </div>
                </div >
            </div>
        
    }
}