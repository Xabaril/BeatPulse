export const 
     statusUp : string = "Up",
     statusDown: string = "Down",
     statusDegraded: string = "Degraded";

const okImage = require("../Assets/images/ok.png");
const downImage = require("../Assets/images/down.png");
const degradedImage = require("../Assets/images/degraded.png");

const imageResources = [
    { state: statusUp, image: okImage },
    { state: statusDown, image: downImage },
    { state: statusDegraded, image: degradedImage },
]

const getStatusImage = (status: string) => imageResources.find(s => s.state == status)!.image;
export { getStatusImage };