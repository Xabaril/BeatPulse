export const
    statusUp: string = "Up",
    statusDown: string = "Down",
    statusDegraded: string = "Degraded";

const okImage = require("../assets/images/ok.png");
const downImage = require("../assets/images/down.png");
const degradedImage = require("../assets/images/degraded.png");
const kubernetesIcon = require('../assets/images/kubernetes-icon.png');

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