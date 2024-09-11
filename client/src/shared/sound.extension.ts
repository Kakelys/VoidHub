export class CustomSound {
    static playMetalPipe() {
        const audio = new Audio()
        audio.src = '../assets/metal-pipe.mp3'
        audio.volume = 0.3

        audio.load()
        audio.play()
    }

    static playReverseBtn() {
        const audio = new Audio()
        audio.src = '../assets/sound_ui_buttonclickrelease.wav'
        audio.volume = 0.3

        audio.load()
        audio.play()
    }
}
