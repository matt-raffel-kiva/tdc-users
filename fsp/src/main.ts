import { Logger } from '@nestjs/common';
import Readline from 'readline';

export class Main {
    private readline: any;

    constructor() {
        this.readline = Readline.createInterface({
            input: process.stdin,
            output: process.stdout,
        });
    }

    public async runAsync(): Promise<any> {
        try {
            Logger.log(`eventually this will do something`);
        } catch (e) {
            Logger.error(e.stack);
            Logger.error('-------Fail-------');
        } finally {
            this.readline.close();
        }
    }
}

async function run() {
    await new Main().runAsync();
}

run();