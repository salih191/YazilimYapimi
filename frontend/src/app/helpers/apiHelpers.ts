import { Dictionary } from "../models/dictionary";

export class ApiUrlHelper {
    static apiUrl:string="https://yazilimyapimi.somee.com/"
    static apiPrefix:string="api/"
    static getUrl(path: string): string {
      return this.apiUrl + this.apiPrefix + path;
    }
  
    static getUrlWithParameters(path: string, parameters: Dictionary[]): string {
      let baseUrl = this.getUrl(path) + '?';
  
      // ?n=John&n=Susan
      for (let i = 0; i < parameters.length; i++) {
        const parameter = parameters[i];
  
        baseUrl += parameter.key;
  
        baseUrl += '=';
  
        baseUrl += parameter.value;
  
        if (i < parameters.length) {
          baseUrl += '&';
        }
      }
      return baseUrl;
    }
  }