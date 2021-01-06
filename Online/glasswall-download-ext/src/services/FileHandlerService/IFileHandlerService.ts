export interface IFileHandlerService {
    apiUrl: string;
    appUri: string;
    rebuildFile(parameters: string): Promise<any>;
}