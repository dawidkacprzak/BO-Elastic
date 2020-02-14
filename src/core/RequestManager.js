import request from "request";

/**
 * @class RequestManager
 * @description Klasa odpowiedzialna za wysyłanie requestów http
 */
export default class RequestManager {
  static clusterIp;
  static setClusterIp = ip => {
    RequestManager.clusterIp = ip;
  };

  /**
   * @function clusterGetPromise
   * @memberof RequestManager
   * @description odpowiada za wysylanie i odbieranie requestu do elastica
   * @param {String} endpoint elastic enpoint. Start with / !
   * @example /_cat/health?format=json/
   */
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
