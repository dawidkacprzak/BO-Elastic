import request from "request";

export default class RequestManager {
  static clusterIp;
  static setClusterIp = ip => {
    RequestManager.clusterIp = ip;
  };

  static clusterGetPromise = endpoint => {
    if (this.clusterIp)
      return new Promise((resolve, reject) => {
        request(
          RequestManager.clusterIp + endpoint,
          (error, response, body) => {
            if (error) {
              reject(error);
            } else {
              resolve(JSON.parse(body));
            }
          }
        );
      });
    else throw Error("Cluster ip is not set!");
  };
}
