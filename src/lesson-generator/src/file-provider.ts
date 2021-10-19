const fs = require('fs');
const {resolve} = require('path');

export default class FileProvider {
    public async isEmpty(directory: string): Promise<boolean> {
        return (await fs.promises.readdir(directory)).length === 0;
    }

    public async ensureDirExists(dir: string): Promise<void> {
        if (!fs.existsSync(dir)) {
            fs.mkdirSync(dir, {recursive: true});
        }
    }

    public async getAllFilesInDirectory(directory: string, filter?: (file: string) => boolean): Promise<string[]> {
        let result: string[] = [];
        for (const sub of await fs.promises.readdir(directory)) {
            const res = resolve(directory, sub);
            if (fs.lstatSync(res).isDirectory()) {
                result = result.concat(await this.getAllFilesInDirectory(res, filter))
            } else if (!filter || filter(sub)) {
                result.push(res);
            }
        }
        return result;
    }

    public async readFile(path: string): Promise<string> {
        return (await fs.promises.readFile(path)).toString();
    }

    public async readFileAsBuffer(path: string): Promise<Buffer> {
        return (await fs.promises.readFile(path));
    }

    public async writeFile(path: string, data: string): Promise<void> {
        await fs.promises.writeFile(path, data);
    }

    public async writeBuffer(path: string, data: Buffer): Promise<void> {
        await fs.promises.writeFile(path, data);
    }

    public async fileExists(path: string): Promise<boolean> {
        return fs.existsSync(path);
    }
}
