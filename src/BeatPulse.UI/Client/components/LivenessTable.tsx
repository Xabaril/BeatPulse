import React from "react";
import { Liveness, Check } from "../typings/models";
import { getStatusImage, statusUp, statusDown, statusDegraded } from "../beatpulseResources";
import { CheckTable } from "./CheckTable";

interface LivenessTableProps {
    livenessData: Array<Liveness>
}

const PlusIcon = require("../../Assets/svg/plus.svg");

export class LivenessTable extends React.Component<LivenessTableProps> {

    constructor(props: LivenessTableProps) {
        super(props);
        this.state = {
            livenessData: props.livenessData
        }

        this.mapTable = this.mapTable.bind(this);
    }

    mapTable(livenessData: Array<Liveness>): Array<Liveness> {

        return livenessData.map(liveness => {
            if (liveness.livenessResult) {
                Object.assign(
                    liveness,
                    { checks: JSON.parse(liveness.livenessResult).checks as Array<Check> })
            }
            return liveness;
        });
    }

    formatDate(date: string) {
        return new Date(date).toLocaleString();
    }

    getStatusPic(status: string) {
        return getStatusImage(status);
    }

    toggleVisibility(event: any) {
        let { currentTarget } = event;
        let checksTable = currentTarget.nextSibling;
        checksTable.classList.contains("hidden") ?
            checksTable.classList.remove("hidden") :
            checksTable.classList.add("hidden");
    }

    render() {
        return <div className="table-responsive">
            <table className="table">
                <thead className="thead-dark">
                    <tr>
                        <th></th>
                        <th>Name</th>
                        <th>IsHealthy</th>
                        <th>Status</th>
                        <th>Last Execution</th>
                    </tr>
                </thead>
                <tbody>
                    {this.mapTable(this.props.livenessData).map((item, index) => {
                        return <React.Fragment>
                            <tr key={index}  onClick={this.toggleVisibility} style={{cursor: 'pointer'}}>
                                <td>
                                 <img className="plus-icon" src={PlusIcon}/>
                                </td>  
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
                            <tr className="checks-table hidden">
                                <td style={{padding: 0}} colSpan={5}>
                                    <CheckTable checks={item.checks}/>
                                </td>
                            </tr>
                        </React.Fragment>
                    })}
                </tbody>
            </table>
        </div>
    }
}

