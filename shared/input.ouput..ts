import util from 'util';
import Readline, { Interface } from 'readline';

const readline: any = Readline.createInterface({
    input: process.stdin,
    output: process.stdout,
});

const question = util.promisify(readline.question);

export class InputOuput {
    constructor() {
    }

    public show(text: string) {
        console.log(text);
    }

    public error(e: any) {
        console.error(e.stack);
        console.error('Application failed');
    }

    public close() {
        console.log();
        readline.close();
    }

    public read() {
        readline.input.read();
    }

    public async ask(prompt: string): Promise<string> {
        let input: string = '';
        await (async () => {
            input = await question(prompt, () => {});
        })();
        return input;
    }
}

