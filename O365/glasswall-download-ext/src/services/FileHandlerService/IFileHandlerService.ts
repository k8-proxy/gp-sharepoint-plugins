export interface IFileHandlerService {
    executeRequest(method: string, endpointUrl: string, headers: string, parameters: string): Promise<any>;
}