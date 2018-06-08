import React from "react";
import { Check } from "../typings/models";

interface CheckTableProps {
    checks: Array<Check>;
}

const CheckTable : React.SFC<CheckTableProps> = (props) => {
    return <table className="table-responsive" style={{display: 'inline-table'}}>
        <thead className="thead-black">
            <tr>
                <th>Name</th>
                <th>Message</th>
                <th>Elapsed time</th>
                <th>Run</th>
                <th>Path</th>
                <th>Is Healthy</th>
            </tr>
        </thead>
        <tbody>
            {props.checks.map((item, index) => {
                return <tr key={index} style={{backgroundColor: '#f6f2f2'}}>
                    <td>
                        {item.name}
                    </td>
                    <td>
                        {item.message}
                    </td>
                    <td>
                        {item.elapsed}
                    </td>
                    <td>
                        {item.run.toString()}
                    </td>
                    <td>
                        {item.path}
                    </td>
                    <td>
                        {item.isHealthy.toString()}
                    </td>
                </tr>
            })}
        </tbody>
    </table>
}

export {CheckTable};