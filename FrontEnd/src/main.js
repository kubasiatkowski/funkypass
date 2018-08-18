const apiUrl = 'https://funkypassdev.azurewebsites.net/api/GeneratePassword?lang=';

  new Vue({
    el: '#funkypass',
    data () {
        return {
          langs: null,
          loading: true,
          lang: 'Random',
          length: 14,
          asciionly: true,
          password: null,
          errors: [],
          success: null,
          errored: false
        }

    },
    methods:{
        checkForm: function (e) {
        e.preventDefault();
        this.success = null;
        this.errors = [];
        reqUrl = apiUrl + encodeURIComponent(this.lang) +
                 "&minlen=" + encodeURIComponent(this.length) +
                 "&asciionly=" + encodeURIComponent(this.asciionly) ; 
        console.log (reqUrl);
        axios
            .get(reqUrl)
            .then(response => {this.password= response.data})
            .catch(error => {
                console.log(error)
                this.errors.push(error)
                this.errored = true
            })
            .finally(() => this.loading = false)
        },
        copyToClipboard: function(e)
        {
            e.preventDefault();
            var copyText = document.getElementById("generatedpass");

            /* Select the text field */
            copyText.select();
          
            /* Copy the text inside the text field */
            try{
                document.execCommand("copy");
                this.success = "Password copied to clipboard."
                console.log(e)
                if (window.getSelection) {window.getSelection().removeAllRanges();}
                else if (document.selection) {document.selection.empty();}
                console.log(this.success)
            }
            catch (error) {
                this.errors.push(error)
                this.errored = true
                console.log(error)
            }
        }
      },     
    mounted () {
      this.errors = [];
      axios
        .get('https://funkypassdev.azurewebsites.net/api/GetLang')
        .then(response => {this.langs = response.data})
        .catch(error => {
            console.log(error)
            this.errors.push(error)
            this.errored = true
        })
        .finally(() => this.loading = false)
    }
  })
 


