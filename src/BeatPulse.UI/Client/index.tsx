import * as React from "react";
import * as ReactDOM from "react-dom";
import { App } from './App';

declare var uiEndpoint : any;
let endpoint = `${window.location.origin}${uiEndpoint}`;

ReactDOM.render(<App endpoint={endpoint} /> , document.getElementById("app"));