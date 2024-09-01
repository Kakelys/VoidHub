import { Pipe, PipeTransform } from '@angular/core'
import { SafeHtml } from '@angular/platform-browser'

@Pipe({
    standalone: true,
    pure: true,
    name: 'reducePost',
})
export class ReducePost implements PipeTransform {
    // skip some tags, that just wrap text
    private ignoredTags = ['span', 'oembed']

    // limit number of specific tags
    private tagLimiter = [{ tag: 'figure', limit: 2 }]

    transform(value: string, symbolLimit: number, tagLimit: number): SafeHtml {
        let result = ''
        let symbolCount = 0
        let tagCount = 0
        let insideTag = false

        let openIndex = 0

        //count open and close tags
        const tagCounter: { tag: string; count: number }[] = []

        let tagName = ''
        let tagNameRead = false

        for (let i = 0; i < value.length; i++) {
            const char = value[i]

            if (char === '<') {
                insideTag = true
                openIndex = i

                // not count closing tags
                if (value[i + 1] !== '/') tagCount++
            } else if (char === '>') {
                insideTag = false

                // check if tag is ignored
                if (this.ignoredTags.indexOf(tagName) !== -1) {
                    tagCount--
                }

                // reduce tag name if is closing tag
                if (tagName.startsWith('/')) {
                    tagName = tagName.substring(1)
                }

                // add to tag counter
                let tagIndex = tagCounter.findIndex((x) => x.tag == tagName)
                if (tagIndex === -1) {
                    tagCounter.push({ tag: tagName, count: 1 })
                    tagIndex = tagCounter.length - 1
                } else {
                    tagCounter[tagIndex].count++
                }

                // skip if limited
                const tagLimitIndex = this.tagLimiter.findIndex((x) => x.tag == tagName)
                if (tagLimitIndex !== -1) {
                    const closeTag = `</${tagName}>`

                    if (tagCounter[tagIndex].count > this.tagLimiter[tagLimitIndex].limit) {
                        // remove last tag and upd index
                        result = result.substring(0, openIndex)
                        i = value.indexOf(closeTag, openIndex) + closeTag.length - 1

                        tagCount--
                        tagNameRead = false
                        tagName = ''
                        continue
                    }
                }

                // reset tag name values
                tagNameRead = false
                tagName = ''
            }

            if (!insideTag) {
                symbolCount++
            } else {
                // read tag name
                if (char !== '<')
                    if (char === ' ' || char === '\n') {
                        tagNameRead = true
                    } else if (!tagNameRead) {
                        tagName += char
                    }
            }

            result += char

            if (symbolCount >= symbolLimit || tagCount > tagLimit) {
                result += '...'
                break
            }
        }

        // Remove the remaining brace if tag limit reached out
        if (tagCount > tagLimit) {
            result = result.substring(0, result.length - 1)
        }

        return result
    }
}
