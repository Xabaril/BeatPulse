export const 
     statusUp : string = "Up",
     statusDown: string = "Down",
     statusDegraded: string = "Degraded";

const okImage = require("../assets/images/ok.png");
const downImage = require("../assets/images/down.png");
const degradedImage = require("../assets/images/degraded.png");

const imageResources = [
    { state: statusUp, image: okImage },
    { state: statusDown, image: downImage },
    { state: statusDegraded, image: degradedImage },
]

const getStatusImage = (status: string) => imageResources.find(s => s.state == status)!.image;
export { getStatusImage };