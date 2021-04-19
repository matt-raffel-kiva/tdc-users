import { Logger } from '@nestjs/common';
import { InputOuput } from '../shared/input.ouput.';

export class Main {
    private ioHandler: InputOuput;

    constructor() {
        this.ioHandler = new InputOuput();
    }

    public run() {
        try {
            this.ioHandler.show(`eventually this will do something`);
            this.ioHandler.read();
            this.ioHandler.show(`eventually this will do something`);
        } catch (e) {
            this.ioHandler.error(e);
        } finally {
            this.ioHandler.close();
        }
    }
}

function run() {
    new Main().run();
}

run();

