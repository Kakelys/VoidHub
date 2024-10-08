/**
 * @license Copyright (c) 2014-2023, CKSource Holding sp. z o.o. All rights reserved.
 * For licensing, see LICENSE.md or https://ckeditor.com/legal/ckeditor-oss-license
 */

import { ClassicEditor } from '@ckeditor/ckeditor5-editor-classic';

import { Alignment } from '@ckeditor/ckeditor5-alignment';
import { Autoformat } from '@ckeditor/ckeditor5-autoformat';
import { Bold, Italic, Underline } from '@ckeditor/ckeditor5-basic-styles';
import { BlockQuote } from '@ckeditor/ckeditor5-block-quote';
import { CodeBlock } from '@ckeditor/ckeditor5-code-block';
import { Essentials } from '@ckeditor/ckeditor5-essentials';
import { FontBackgroundColor, FontColor, FontFamily, FontSize } from '@ckeditor/ckeditor5-font';
import { Highlight } from '@ckeditor/ckeditor5-highlight';
import {
	AutoImage,
	Image,
	ImageCaption,
	ImageInsertViaUrl,
	ImageResize,
	ImageStyle,
	ImageToolbar
} from '@ckeditor/ckeditor5-image';
import { Indent, IndentBlock } from '@ckeditor/ckeditor5-indent';
import { AutoLink, Link } from '@ckeditor/ckeditor5-link';
import { List } from '@ckeditor/ckeditor5-list';
import { MediaEmbed } from '@ckeditor/ckeditor5-media-embed';
import { Paragraph } from '@ckeditor/ckeditor5-paragraph';
import { RemoveFormat } from '@ckeditor/ckeditor5-remove-format';
import { SelectAll } from '@ckeditor/ckeditor5-select-all';
import {
	Table,
	TableCellProperties,
	TableColumnResize,
	TableProperties,
	TableToolbar
} from '@ckeditor/ckeditor5-table';

// custom styles
import '../src/custom.css';
import { environment as env }  from '../../src/environments/environment';

// fast build and start
// cd ./ckeditor5 && npm run build && cd .. && ng serve

// You can read more about extending the build with additional plugins in the "Installing plugins" guide.
// See https://ckeditor.com/docs/ckeditor5/latest/installation/plugins/installing-plugins.html for details.

class Editor extends ClassicEditor {
	public static override builtinPlugins = [
		Alignment,
		AutoImage,
		AutoLink,
		Autoformat,
		BlockQuote,
		Bold,
		CodeBlock,
		Essentials,
		FontBackgroundColor,
		FontColor,
		FontFamily,
		FontSize,
		Highlight,
		Image,
		ImageCaption,
		ImageResize,
		ImageStyle,
		ImageToolbar,
    ImageInsertViaUrl,
		Indent,
		IndentBlock,
		Italic,
		Link,
		List,
		MediaEmbed,
		Paragraph,
		RemoveFormat,
		SelectAll,
		Table,
		TableCellProperties,
		TableColumnResize,
		TableProperties,
		TableToolbar,
		Underline
	];

	public static override defaultConfig = {
		toolbar: {
			items: [
				'bold',
				'italic',
				'underline',
				'highlight',
				'|',
				'fontSize',
				'fontFamily',
				'fontColor',
				'fontBackgroundColor',
				'alignment',
				'|',
				'bulletedList',
				'numberedList',
				'|',
				'removeFormat',
				'-',
				'link',
				'codeBlock',
				'blockQuote',
				'mediaEmbed',
        'insertImage',
				'insertTable'
			],
			shouldNotGroupWhenFull: true
		},
		language: 'en',
		image: {
      //resize
      resizeUnit: "px" as "px",
      resizeOptions: [
          {
              name: 'imageResize:original',
              value: null,
              icon: 'original'
          },
          {
              name: 'imageResize:50',
              value: '50',
              icon: 'small'
          },
          {
            name: 'imageResize:75',
            value: '75',
            icon: 'medium'
          },
          {
              name: 'imageResize:100',
              value: '100',
              icon: 'large'
          }
      ],

      //
      toolbar: [
        'imageTextAlternative',
        'toggleImageCaption',
        '|',
        'imageStyle:breakText',
        'imageStyle:wrapText',
        '|',
        'imageResize',
      ]
		},
		table: {
			contentToolbar: [
				'tableColumn',
				'tableRow',
				'mergeTableCells',
				'tableCellProperties',
				'tableProperties'
			]
		},
    mediaEmbed: {
      // git with default providers config:
      // https://github.com/ckeditor/ckeditor5/blob/master/packages/ckeditor5-media-embed/src/mediaembedediting.ts
      // cd .\ckeditor5\ && npm run build && cd .. && npm install ./ckeditor5/ && ng serve
      providers: [
        {
          name: "youtube-nocookie",
          url: [
						/^(?:m\.)?youtube\.com\/watch\?v=([\w-]+)(?:&t=(\d+))?/,
						/^(?:m\.)?youtube\.com\/v\/([\w-]+)(?:\?t=(\d+))?/,
						/^youtube\.com\/embed\/([\w-]+)(?:\?start=(\d+))?/,
						/^youtu\.be\/([\w-]+)(?:\?t=(\d+))?/
					],
          html: (match:any) => {
            const id = match[ 1 ];
            const time = match[ 2 ];

            return (
              '<div>' +
                `<iframe src="https://www.youtube-nocookie.com/embed/${ id }${ time ? `?start=${ time }` : '' }" ` +
                  'style="width: 100%; min-height: 400px;"' +
                  'frameborder="0" allow="autoplay; encrypted-media" allowfullscreen loading="lazy">' +
                '</iframe>' +
              '</div>'
            );
          }
        },
        {
          name: "twitter",
          url: [
            /^twitter\.com\/(.+)\/status\/(\d+)/
          ],
          html: (match:any) => {
            const user = match[ 1 ];
            const id = match[ 2 ];

            return (
            '<div class="twitter-container"> ' +
              '<iframe ' +
                `src="https://twitframe.com/show?url=https://twitter.com/${user}/status/${id}"` +
                'style="height:400px" csp frameborder="0" allowtransparency="true" allow="encrypted-media" loading="lazy"' +
              '/> ' +
            '</div>'
            );
          }
        },
        {
          name: "apivideo",
          url: [
            /^.*\/images\/posts\/([0-9a-fA-F]+-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}).mp4/,
          ],
          html: (match:any) => {
            const src = match[ 1 ];

            return (
              "<video style='width: 100%; min-height: 400px; max-height: 450px;' controls='controls' preload='metadata'>" +
                `<source src="${env.resourceURL}/images/posts/${src}.mp4" type="video/mp4">` +
              "</video>"
            );
          }
        }
      ],
    }
	};
}

export default Editor;
