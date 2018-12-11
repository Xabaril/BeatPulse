export const
    statusUp: string = "Up",
    statusDown: string = "Down",
    statusDegraded: string = "Degraded";

const okImage = require("../Assets/images/ok.png");
const downImage = require("../Assets/images/down.png");
const degradedImage = require("../Assets/images/degraded.png");
const kubernetesIcon = require('../Assets/images/kubernetes-icon.png');

const imageResources = [
    { state: statusUp, image: okImage },
    { state: statusDown, image: downImage },
    { state: statusDegraded, image: degradedImage },
]

export const discoveryServices = [
    { name: 'kubernetes', image: kubernetesIcon }
];

const getStatusImage = (status: string) => imageResources.find(s => s.state == status)!.image;
export { getStatusImage };