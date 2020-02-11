import request from 'request'

export default class RequestManager{
    static clusterIp;

    constructor(clusterIp){
        RequestManager.clusterIp = clusterIp;
    }

    setClusterIp = (ip) => {
        RequestManager.clusterIp = ip;
    }

    clusterGetPromise = (endpoint) => {
        return new Promise((resolve,reject) => {
            request(RequestManager.clusterIp, (error, response, body) => {
                if(error){
                    reject(error);
                }else{
                    resolve(JSON.parse(body))
                }
            })
        });
    }
}