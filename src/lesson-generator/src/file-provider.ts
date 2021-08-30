const fs = require('fs');
const {resolve} = require('path');

export default class FileProvider {
    public async isEmpty(directory: string): Promise<boolean> {
        return (await fs.promises.readdir(directory)).length === 0;
    }

    public async getAllFilesInDirectory(directory: string, filter?: (file: string) => boolean): Promise<string[]> {
        let result: string[] = [];
        for (const sub of await fs.promises.readdir(directory)) {
            const res = resolve(directory, sub);
            if (await fs.promises.stat(res).isDirectory()) {
                result = result.concat(await this.getAllFilesInDirectory(sub, filter))
            } else if (!filter || filter(sub)) {
                result.push(sub);
            }
        }
        return result;
    }

    public async readFile(path: string): Promise<string> {
        return fs.promises.readFile(path);
    }

    public async writeFile(path: string, data: string): Promise<void> {
        await fs.promises.writeFile(path, data);
    }
}