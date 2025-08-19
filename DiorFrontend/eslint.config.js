// ESLint flat config (ESM) for Angular + TS
import js from '@eslint/js';
import tseslint from 'typescript-eslint';

export default [
  {
    ignores: ['dist/**', 'node_modules/**', '**/*.html']
  },
  {
    files: ['**/*.ts'],
    languageOptions: {
      parser: tseslint.parser,
      parserOptions: {
        ecmaVersion: 2022,
        sourceType: 'module'
      },
      globals: {
        window: 'readonly',
        document: 'readonly',
        console: 'readonly',
        localStorage: 'readonly',
        sessionStorage: 'readonly',
        setTimeout: 'readonly',
        setInterval: 'readonly',
        clearTimeout: 'readonly',
        confirm: 'readonly',
        alert: 'readonly',
        prompt: 'readonly',
        URL: 'readonly',
        File: 'readonly',
        Blob: 'readonly',
        process: 'readonly',
        navigator: 'readonly',
        ErrorEvent: 'readonly',
        MouseEvent: 'readonly',
        Node: 'readonly',
        HTMLInputElement: 'readonly',
        Event: 'readonly',
        KeyboardEvent: 'readonly',
        WebSocket: 'readonly',
        MessageEvent: 'readonly',
        btoa: 'readonly',
        atob: 'readonly'
      }
    },
    plugins: { '@typescript-eslint': tseslint.plugin },
    rules: {
      ...js.configs.recommended.rules,
      'no-unused-vars': 'off',
      'no-console': 'off',
      'no-irregular-whitespace': 'warn',
      // Autoriser les catch vides (ex: try { ... } catch {})
      'no-empty': ['error', { allowEmptyCatch: true }],
      // Certaines chaînes incluent des échappements pour des modèles; éviter le bruit
      'no-useless-escape': 'off'
    }
  },
  
];
