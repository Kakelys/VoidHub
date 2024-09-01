module.exports = {
    parser: '@typescript-eslint/parser',
    parserOptions: {
        ecmaVersion: 2021,
        sourceType: 'module',
        ecmaFeatures: {
            jsx: true,
        },
    },
    settings: {
        angular: {
            version: 'detect',
        },
    },
    env: {
        browser: true,
        node: true,
    },
    extends: [
        'eslint:recommended',
        'prettier/@typescript-eslint',
        'plugin:@typescript-eslint/recommended',
    ],
}
