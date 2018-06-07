import React from "react";
import { Switch, Route, Link } from "react-router-dom";
import { LivenessPage } from "./components/LivenessPage";
import { WebhooksPage } from "./components/WebhooksPage";
import { scaleRotate as Menu } from "react-burger-menu";
interface AppProps {
    mountPath: string;
    apiEndpoint: string;
    webhookEndpoint: string;
}

const App: React.SFC<AppProps> = (props) => {
    return <React.Fragment>
        <div id="outer-container" style={{ height: '100%' }}>
            <Menu pageWrapId={'wrapper'} outerContainerId={"outer-container"} >
                <Link to={props.mountPath} className="menu-item">Liveness</Link>
                <Link to="/webhooks" className="menu-item">Webhooks</Link>
                <Link to="/aboutus" className="menu-item">About us</Link>
            </Menu>
            <Route exact path={props.mountPath} render={() => <LivenessPage endpoint={props.apiEndpoint} />} />
            <Route path="/webhooks" render={() => <WebhooksPage endpoint={props.webhookEndpoint} />} />
        </div>
    </React.Fragment>
}

export { App };