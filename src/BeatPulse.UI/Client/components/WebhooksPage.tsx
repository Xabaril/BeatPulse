import React from "react";
import { BeatPulseClient } from "../beatpulseClient";
import { WebHook } from "../typings/models";
import ReactJson from "react-json-view";
import { chunkArray } from "../utils/array";
interface WebhooksPageProps {
    endpoint: string;
}
interface WebHooksPageState {
    webhooks: Array<WebHook>;
}
const GearIcon = require('../../Assets/svg/gear.svg');

export class WebhooksPage extends React.Component<WebhooksPageProps, WebHooksPageState> {
    private _beatpulseClient: BeatPulseClient;
    constructor(props: WebhooksPageProps) {
        super(props);
        this._beatpulseClient = new BeatPulseClient(props.endpoint);
        this.state = {
            webhooks: []
        }
    }
    componentDidMount() {
        this.getWebhooks();
    }

    async getWebhooks() {
        const webhooks = (await this._beatpulseClient.getData()).data as Array<WebHook>;
        this.setState({
            webhooks
        });
    }
    renderWebhooks(webhooks: Array<WebHook>) {
        let webHooksChunk = chunkArray(webhooks, 2);
        let components: any[] = [];
        for (let chunkWebhooks of webHooksChunk) {
            var component = <div className="row">
                {chunkWebhooks.map((webhook, index) => {
                    return <div className="col-md-6">
                        <div className="webhook">
                            <span className="block"><b>Name</b> : {webhook.name}</span>
                            <span className="block"><b>Uri</b> : {webhook.uri}</span>
                            <span className="block"><b>Payload</b> :</span>
                            <ReactJson theme="solarized" src={webhook.payload as Object} />
                        </div>
                    </div>
                })}
             </div>
             components.push(component);
        }
        return components;
    }
    render() {
        return <div id="wrapper" style={{ height: '100%', overflow: 'auto' }}>
            <div className="container webhook-container">
                <div className="webhooks">                    
                    <h2 className="title" style={{ marginLeft: '1.35%', marginTop: '5%' }}>
                    <img className="gear-icon" src={GearIcon}/>
                    {this.state.webhooks.length} Configured Webhooks
                    </h2>
                    {this.renderWebhooks(this.state.webhooks)}                    
                </div>
            </div>
        </div>
    }
}